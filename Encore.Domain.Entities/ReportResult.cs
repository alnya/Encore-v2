﻿namespace Encore.Domain.Entities
{
    using System;
    using System.Collections.Generic;

    public class ReportResult : EntityBase
    {
        public Guid ReportId { get; set; }

        public string ReportName { get; set; }

        public List<string> FieldIds;

        public Guid RequestedBy { get; set; }

        public DateTime RunDate { get; set; }
    }
}
