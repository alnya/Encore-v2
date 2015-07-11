namespace Encore.Domain.Entities.BusinessObjects
{
    using Encore.Domain.Entities.BusinessObjects;
    using System.Collections.Generic;
    
    public class ReportResultsResponse
    {
        public string ReportName { get; set; }

        public long Pages { get; set; }

        public long Count { get; set; }

        public IEnumerable<ReportResultColumn> FieldColumns { get; set; }

        public IEnumerable<ReportResultRow> Rows { get; set; }
    }
}
