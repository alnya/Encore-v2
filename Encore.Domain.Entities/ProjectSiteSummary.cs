namespace Encore.Domain.Entities
{
    using System;
    
    public class ProjectSiteSummary
    {
        public int FieldSourceId { get; set; }

        public int SiteSourceId { get; set; }

        public DateTime ValueMaxDate { get; set; }

        public DateTime ValueMinDate { get; set; }

        public int RowCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
