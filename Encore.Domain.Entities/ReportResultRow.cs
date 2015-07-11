namespace Encore.Domain.Entities
{
    using System;
    using System.Collections.Generic;

    public class ReportResultRow : EntityBase
    {
        public ReportResultRow()
        {
            Fields = new List<ReportResultField>();
        }

        public Guid ReportResultId { get; set; }

        public string SiteName { get; set; }

        public int SiteId { get; set; }

        public DateTime RowDateTime { get; set; }

        public List<ReportResultField> Fields { get; set; }
    }
}
