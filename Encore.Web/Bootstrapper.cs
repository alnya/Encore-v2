namespace Encore.Web
{
    using AutoMapper;
    using Encore.DataStore;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Services;
    using Encore.Web.Mapping;
    using Encore.Web.Models;
    using log4net;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.Json;
    using Nancy.Session;
    using Nancy.TinyIoc;
    using System.Configuration;
    using System.Reflection;
    using Encore.Web.Extensions;
    using Encore.PoolParty;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly ILog log = LogManager.GetLogger(typeof(Bootstrapper));

        private IRepositoryContext repositoryContext;
        private IProvideFieldData poolPartyClient;

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            JsonSettings.RetainCasing = true;
            JsonSettings.MaxJsonLength = int.MaxValue;
            StaticConfiguration.CaseSensitive = false;

            var sessionManager = new SessionManager(new MemorySessionProvider("Encore", int.Parse(ConfigurationManager.AppSettings["Session.Timeout"])));
            sessionManager.Run(pipelines);

            pipelines.OnError.AddItemToEndOfPipeline((ctx, ex) =>
            {
                log.Error("Unhandled error on request: " + ctx.Request.Url, ex);

                ctx.Items["error"] = new Error
                {
                    ErrorMessage = "An unhandled error occurred",
                    StatusCode = HttpStatusCode.InternalServerError
                };

                return null;
            });

            ConfigureAutoMapper();
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
         
            repositoryContext = new MongoContext(
                ConfigurationManager.AppSettings["ConnectionString"], "Encore");

            poolPartyClient = new PoolPartyClient(
                ConfigurationManager.AppSettings["PoolParty.EncoreUrl"],
                ConfigurationManager.AppSettings["PoolParty.UserName"],
                ConfigurationManager.AppSettings["PoolParty.Password"]);

            container.Register<IRepositoryContext>(repositoryContext);
            container.Register<IProvideFieldData>(poolPartyClient);
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);
            container.Register(Mapper.Engine);
            container.Register<IUserService, UserService>();
            container.Register<IReportService, ReportService>();
            container.Register<IReportResultService, ReportResultService>();
            container.Register<IProjectService, ProjectService>();
            container.Register<ISiteService, SiteService>();
            container.Register<IFieldService, FieldService>();
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            EnsureAdminUser(container);

            pipelines.BeforeRequest.AddItemToEndOfPipeline(ctx =>
            {
                if (ctx.CurrentUser == null)
                {
                    ctx.SetupIdentityFromSession();
                }

                return null;
            });
        }
                
        private void EnsureAdminUser(TinyIoCContainer container)
        {
            var userService = container.Resolve<IUserService>();
            userService.EnsureAdminUser();
        }

        private static void ConfigureAutoMapper()
        {
            var types = Assembly.GetExecutingAssembly().GetExportedTypes();
            AutoMapperConfiguration.Configure(Mapper.Configuration, types);
        }
    }
}