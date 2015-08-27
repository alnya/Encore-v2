namespace Encore.Web.Models
{
    using Encore.Web.Mapping;
    using System;

    [MapsFrom(typeof(Domain.Entities.BusinessObjects.ReporDataSummary))]
    public class ReportDataSummary
    {
        public DateTime dateFrom { get; set; }

        public DateTime dateTo { get; set; }

        public bool dataAvailable { get; set; }
    }
}
