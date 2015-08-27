namespace Encore.Domain.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Services.Search;
    using System;
    using System.Collections.Generic;
    using System.Linq;
        
    public class SiteService : ISiteService
    {
        private readonly IRepositoryContext context;

        public SiteService(IRepositoryContext context)
        {
            this.context = context;
        }

        public IPagedListResult<Site> Search(IRequestedPage requestedPage, ISortCriteria sortCriteria = null, ISearchTerms searchTerms = null)
        {
            var siteRepo = context.GetRepository<Site>();

            var results = siteRepo.Search(searchTerms, null, sortCriteria, requestedPage);
            var count = siteRepo.Count(searchTerms);

            return new PagedListResult<Site>(results, count);
        }

        public bool ReplaceSites(ICollection<Site> sites)
        {
            var siteRepo = context.GetRepository<Site>();
            var currentSites = siteRepo.GetWhere();
            siteRepo.DeleteAll();

            foreach (var site in sites)
            {
                var currentSite = currentSites.FirstOrDefault(x => String.Equals(x.Name, site.Name, StringComparison.OrdinalIgnoreCase));

                if (currentSite != null)
                {
                    site.Id = currentSite.Id;
                }

                siteRepo.Save(site);
            }

            return true;
        }

        public bool DeleteAll()
        {
            var siteRepo = context.GetRepository<Site>();
            siteRepo.DeleteAll();

            return true;
        }
    }
}
