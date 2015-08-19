namespace Encore.Web.Modules
{
    using Encore.Web.Models;
    using AutoMapper;
    using Extensions;

    public class PagesModule : BaseModule
    {

        public PagesModule(IMappingEngine mappingEngine)
            : base("", mappingEngine)
        {
            Get["/"] = GetLogin;

            Get["/login"] = GetLogin;

            Get["/login/create"] = CreateLogin;     
        }

        private dynamic GetLogin(dynamic args)
        {
            Context.ClearSession();

            return View["login", new PageModel { Title = "Login", ViewModel = "login"}];
        }

        private dynamic CreateLogin(dynamic args)
        {
            return View["createLogin", new PageModel { Title = "Create Account", ViewModel = "user" }];
        }
    }
}
