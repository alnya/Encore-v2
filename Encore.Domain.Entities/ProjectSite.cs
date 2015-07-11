namespace Encore.Domain.Entities
{
    using System;

    public class ProjectSite
    {
        public int SourceId { get; set; }
        
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
