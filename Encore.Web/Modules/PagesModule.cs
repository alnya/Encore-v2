namespace Encore.Web.Modules
{
    using Nancy;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Nancy.ModelBinding;
    using Nancy.Security;
    using Encore.Domain.Interfaces.Services;
    using Encore.Web.Models;
    using Encore.Domain.Entities;
    using AutoMapper;
    using Extensions;
    using Nancy.Session;
    
    public class PagesModule : BaseModule
    {
        private readonly IUserService userService;

        public PagesModule(IUserService userService, IMappingEngine mappingEngine)
            : base("", mappingEngine)
        {
            this.userService = userService;

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
