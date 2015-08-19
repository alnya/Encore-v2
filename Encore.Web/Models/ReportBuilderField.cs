namespace Encore.Web.Models
{
    using Encore.Web.Mapping;
    using System.Collections.Generic;

    [MapsFrom(typeof(Domain.Entities.BusinessObjects.ReportBuilderField))]
    public class ReportBuilderField
    {
        public string id {get; set; }

        public string name { get; set; }

        public string type { get; set; }

        public IEnumerable<string> altNames { get; set; }

        public IEnumerable<string> siteIds { get; set; }

        public IEnumerable<string> projects { get; set; }
    }
}
