namespace Encore.Web.Modules
{
    using AutoMapper;
    using System.Linq;
    using Encore.Web.Models;
    using Extensions;
    using Nancy.Responses;
    using System.Collections.Generic;
    using Nancy;

    public class SecurePagesModule : BaseModule
    {
        public SecurePagesModule(IMappingEngine mappingEngine)
            : base("pages", mappingEngine)
        {
            Before += ctx => (this.Context.CurrentUser == null) ? new RedirectResponse("/") : null;

            Get["/"] = GetHome;

            Get["/reports"] = GetReports;

            Get["/reports/{id}"] = GetReport;

            Get["/reports/results/{id}"] = GetReportResults;

            Get["/projects"] = GetProjects;

            Get["/projects/{id}"] = GetProject;

            Get["/users"] = GetUsers;

            Get["/users/{id}"] = GetUser;

            Get["/setup"] = GetSetup;

            Get["/myAccount"] = GetMyAccount;

            Get["/logout"] = args =>
            {
                Context.ClearSession();
                return new RedirectResponse("/");
            };
        }

        private dynamic GetHome(dynamic args)
        {
            return View["home", Vm(args, "Home")];
        }

        private dynamic GetReports(dynamic args)
        {
            return View["reports", Vm(args, "Reports")];
        }

        private dynamic GetReport(dynamic args)
        {
            return View["report", Vm(args, "Report")];
        }

        private dynamic GetReportResults(dynamic args)
        {
            return View["results", Vm(args, "Results")];
        }

        private dynamic GetProjects(dynamic args)
        {
            if(!IsAdminUser())
            {
                RespondWithUnauthorized();
            }

            return View["list", Vm(args, "Projects")];
        }

        private dynamic GetProject(dynamic args)
        {
            if (!IsAdminUser())
            {
                RespondWithUnauthorized();
            }

            return View["project", Vm(args, "Project")];
        }

        private dynamic GetUsers(dynamic args)
        {
            if (!IsAdminUser())
            {
                RespondWithUnauthorized();
            }

            return View["list", Vm(args, "Users")];
        }

        private dynamic GetUser(dynamic args)
        {
            if (!IsAdminUser())
            {
                RespondWithUnauthorized();
            }

            return View["user", Vm(args, "User")];
        }

        private dynamic GetSetup(dynamic args)
        {
            if (!IsAdminUser())
            {
                RespondWithUnauthorized();
            }

            return View["setup", Vm(args, "Setup")];
        }

        private dynamic GetMyAccount(dynamic args)
        {
            return View["myAccount", Vm(args, "MyAccount")];
        }

        private bool IsAdminUser()
        {
            return Context.CurrentUser.Claims.Any(x => x == "Admin");
        }

        private PageModel Vm(dynamic args, string view)
        {
            var vm = new PageModel() 
            {
                Username = Context.CurrentUser.UserName,
                Title = view,
                ViewModel = view.ToLower(),
            };

            if (args.id)
            {
                vm.RecordId = args.id;
            }

            var navPages = new List<dynamic> { new { Name = "Reports", Icon = "glyphicon-file" } };

            if (IsAdminUser())
            {
                navPages.Add(new { Name = "Projects", Icon = "glyphicon-globe" });
                navPages.Add(new { Name = "Users", Icon = "glyphicon-user" });
                navPages.Add(new { Name = "Setup", Icon = "glyphicon-cog" });
            }

            foreach (var navPage in navPages)
            {
                vm.Pages.Add(new PageModel.NavigationPage
                {
                    Url = navPage.Name.ToLower(),
                    Title = navPage.Name,
                    Icon = navPage.Icon,
                    Current = Context.Request.Url.Path.ToLower().IndexOf(navPage.Name.ToLower()) > -1
                });
            }

            return vm;
        }
    }
}
