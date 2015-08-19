namespace Encore.Web.Modules
{
    using AutoMapper;
    using Encore.Domain.Interfaces.Services;
    using Nancy;
    using System;
    using Extensions;
    using Encore.Domain.Entities;
    using Nancy.ModelBinding;

    public class ProjectModule : SecureModule
    {
        private readonly IProjectService projectService;

        public ProjectModule(IProjectService projectService, IMappingEngine mappingEngine)
            : base("data/projects", mappingEngine)
        {
            this.projectService = projectService;

            Get["/"] = SearchProjects;

            Get["/{id}"] = GetProject;

            Post["/"] = AddProject;

            Put["/{id}"] = UpdateProject;

            Delete["/{id}"] = DeleteProject;

            Put["/testUrl"] = TestProjectUrl;

            Put["/{id}/syncDetails"] = SyncDetails;
        }

        private dynamic AddProject(dynamic args)
        {
            var model = this.BindAndValidate<Encore.Web.Models.Project>();

            if (!ModelValidationResult.IsValid)
            {
                return RespondWithValidationFailure(ModelValidationResult);
            }

            var addEntity = MapTo<Project>(model);
            var returnEntity = projectService.Add(addEntity);

            return MapTo<Encore.Web.Models.Project>(returnEntity);
        }

        private dynamic UpdateProject(dynamic args)
        {
            var model = this.BindAndValidate<Encore.Web.Models.Project>();

            if (!ModelValidationResult.IsValid)
            {
                return RespondWithValidationFailure(ModelValidationResult);
            }

            var updateEntity = MapTo<Project>(model);
            var returnEntity = projectService.Update(new Guid(args.id), updateEntity);

            return MapTo<Encore.Web.Models.Project>(returnEntity);
        }

        private dynamic SearchProjects(dynamic args)
        {
            var entities = projectService.Search(
                Context.RequestedPage(),
                null,
                Context.SortCriteria<Project>(),
                Context.SearchTerms<Project>());

            return MapToResultList<Project, Encore.Web.Models.Project>(entities);
        }

        private dynamic GetProject(dynamic args)
        {
            var entity = projectService.Get(new Guid(args.id));

            if (entity == null)
            {
                return Negotiate.WithStatusCode(HttpStatusCode.NotFound);
            }

            return MapTo<Encore.Web.Models.Project>(entity);
        }

        private dynamic DeleteProject(dynamic args)
        {
            return projectService.Delete(new Guid(args.id));
        }

        private dynamic TestProjectUrl(dynamic args)
        {
            var model = this.Bind<Encore.Web.Models.Project>();

            return projectService.TestProjectUrl(model.ApiUrl);
        }

        private dynamic SyncDetails(dynamic args)
        {
            var projectId = new Guid(args.id);

            projectService.SyncProjectFields(projectId);
            projectService.SyncProjectSites(projectId);

            return true;
        }
    }
}
