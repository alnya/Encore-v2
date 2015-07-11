namespace Encore.Domain.Entities.BusinessObjects
{
    using System;
    
    public class ReportResultAvailable
    {
        public string ReportName { get; set; }

        public Guid ResultId { get; set; }

        public DateTime RequestDate { get; set; }
    }
}
