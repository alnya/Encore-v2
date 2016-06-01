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
        }

        private dynamic GetLogin(dynamic args)
        {
            Context.ClearSession();

            return View["login", new PageModel { Title = "Login", ViewModel = "login"}];
        }
    }
}
