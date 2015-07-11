namespace Encore.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    public class Field : EntityBase
    {
        public string SourceId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Unit { get; set; }

        public List<int> ProjectIds { get; set; }
    }
}
