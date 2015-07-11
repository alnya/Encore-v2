namespace Encore.Domain.Entities.BusinessObjects
{
    using System.Collections.Generic;

    public class ReportBuilderData
    {
        public List<string> SiteTypes { get; set; }

        public List<string> FieldTypes { get; set; }

        public List<string> Projects { get; set; }

        public List<ReportBuilderSite> SiteData { get; set; }

        public List<ReportBuilderField> FieldData { get; set; }
    }
}
