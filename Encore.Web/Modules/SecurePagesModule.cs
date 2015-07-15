namespace Encore.Web.Modules
{
    using AutoMapper;
    using Encore.Web.Models;
    using Extensions;
    using Nancy.Responses;

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

            Get["/setup"] = GetSetup;

            Get["/account"] = GetAccount;

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
            return View["list", Vm(args, "Projects")];
        }

        private dynamic GetProject(dynamic args)
        {
            return View["project", Vm(args, "Project")];
        }

        private dynamic GetSetup(dynamic args)
        {
            return View["setup", Vm(args, "Setup")];
        }

        private dynamic GetAccount(dynamic args)
        {
            return View["account", Vm(args, "User")];
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

            var navPages = new dynamic[] 
            {
                new { Name = "Projects", Icon = "glyphicon-globe" }, 
                new { Name = "Reports", Icon = "glyphicon-file" },
                new { Name = "Setup", Icon = "glyphicon-cog" },
            };

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
