namespace Encore.Web.Models
{
    using Encore.Web.Mapping;
    using System;

    [MapsFrom(typeof(Domain.Entities.BusinessObjects.ReportResultAvailable))]
    public class ReportResultAvailable
    {
        public string ReportName { get; set; }

        public Guid ResultId { get; set; }

        public DateTime RequestDate { get; set; }
    }
}
