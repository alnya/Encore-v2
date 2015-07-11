namespace Encore.Web.Models
{
    using Encore.Web.Mapping;
    using System;
    using System.Collections.Generic;

    [MapsFrom(typeof(Domain.Entities.BusinessObjects.ReportBuilderData))]
    public class ReportBuilderData
    {
        public IEnumerable<string> siteTypes { get; set; }

        public IEnumerable<string> fieldTypes { get; set; }

        public IEnumerable<string> projects { get; set; }

        public IEnumerable<ReportBuilderField> fieldData { get; set; }

        public IEnumerable<ReportBuilderSite> siteData { get; set; }
    }
}
