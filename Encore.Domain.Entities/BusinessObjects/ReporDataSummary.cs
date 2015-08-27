namespace Encore.Domain.Entities.BusinessObjects
{
    using System;
    using System.Collections.Generic;

    public class ReporDataSummary
    {
        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public bool DataAvailable { get; set; }
    }
}
