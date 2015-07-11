namespace Encore.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    public class ReportRequest : EntityBase
    {
        public Guid ReportId { get; set; }

        public DateTime RequestDate { get; set; }

        public RequestStatus Status { get; set; }
    }
}
