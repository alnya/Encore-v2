namespace Encore.Domain.Services.Extensions
{
    using Encore.Domain.Entities;
    using System.Collections.Generic;

    public static class SiteExtensions
    {
        public static Dictionary<int, string> ToProjectIdMap(this IEnumerable<ProjectSite> sites)
        {
            var siteMap = new Dictionary<int, string>();

            foreach (var site in sites)
            {
                siteMap.Add(site.SourceId, site.Name);
            }

            return siteMap;
        }
    }
}
