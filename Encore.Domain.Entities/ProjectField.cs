namespace Encore.Domain.Entities
{
    using System;

    public class ProjectField
    {
        public Guid FieldId { get; set; }

        public int SourceId { get; set; }

        public string Name {get; set; }

        public string Unit { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}