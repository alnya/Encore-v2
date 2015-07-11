namespace Encore.Web.Models
{
    using Encore.Web.Mapping;
    using System.Collections.Generic;

    [MapsFrom(typeof(Domain.Entities.BusinessObjects.ReportBuilderSite))]
    public class ReportBuilderSite
    {
        public string id { get; set; }

        public string name { get; set; }

        public string type { get; set; }

        public IEnumerable<string> fieldIds { get; set; }

        public IEnumerable<string> projects { get; set; }
    }
}
