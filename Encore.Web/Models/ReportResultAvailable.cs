namespace Encore.Web.Models
{
    using Encore.Web.Mapping;
    using System;

    [MapsFrom(typeof(Domain.Entities.ReportResult))]
    public class ReportResultAvailable
    {
        public string ReportName { get; set; }

        public Guid Id { get; set; }

        public DateTime RunDate { get; set; }
    }
}
