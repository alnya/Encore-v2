namespace Encore.Web.Modules
{
    using AutoMapper;
    using Extensions;
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.Services;
    using System.Collections.Generic;
    
    public class SetupModule : SecureModule
    {
        private readonly IProjectService projectService;

        private readonly ISiteService siteService;

        private readonly IFieldService fieldService;

        public SetupModule(IProjectService projectService, ISiteService siteService, IFieldService fieldService, IMappingEngine mappingEngine)
            : base("data/setup", mappingEngine)
        {
            this.projectService = projectService;
            this.siteService = siteService;
            this.fieldService = fieldService;

            Get["/sites"] = SearchSites;

            Put["/sites"] = UpdateSites;

            Put["/syncProjectSummaries"] = SyncProjectSummaries;

            Put["/syncFields"] = SyncFields;
        }

        private dynamic SearchSites(dynamic args)
        {
            var entities = siteService.Search(
                Context.RequestedPage(),
                Context.SortCriteria<Site>(),
                Context.SearchTerms<Site>());

            return MapToResultList<Site, Encore.Web.Models.Site>(entities);
        }

        private dynamic UpdateSites(dynamic args)
        {
            var models = this.BindAndValidateEnumerable<IEnumerable<Encore.Web.Models.Site>>();

            if (!ModelValidationResult.IsValid)
            {
                return RespondWithValidationFailure(ModelValidationResult);
            }

            var sites = MapTo<IEnumerable<Site>>(models);
            siteService.ReplaceSites(new List<Site>(sites));
            return true;
        }

        private dynamic SyncProjectSummaries(dynamic args)
        {
            projectService.SyncAllProjectSummaries();
            return true;
        }

        private dynamic SyncFields(dynamic args)
        {
            fieldService.SyncFields();
            return true;
        }
    }
}
