namespace Encore.Domain.Entities
{
    using System.Collections.Generic;
    
    public class Field : EntityBase
    {
        public string SourceId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Unit { get; set; }

        public List<int> ProjectIds { get; set; }
    }
}
