namespace Encore.Domain.Entities
{
    using System;
    
    public class ReportRequest : EntityBase
    {
        public Guid ReportId { get; set; }

        public DateTime RequestDate { get; set; }

        public RequestStatus Status { get; set; }
    }
}
