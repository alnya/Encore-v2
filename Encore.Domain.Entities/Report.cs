namespace Encore.Domain.Entities
{
    using System;
    using System.Collections.Generic;

    public class Report : EntityBase
    {
        public string Name { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public List<string> FieldIds;

        public List<Guid> SiteIds;

        public Guid CreatedBy { get; set; }

        public DateTime LastRequested { get; set; }

        public RequestStatus LastRequestStatus { get; set; }

        public Guid? LastResultId { get; set; }
    }
}
