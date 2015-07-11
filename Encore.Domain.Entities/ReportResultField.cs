namespace Encore.Domain.Entities
{
    using System;
    
    public class ReportResultField
    {
        public string FieldId { get; set; }

        public int ProjectId { get; set; }

        public string Value { get; set; }
    }
}
