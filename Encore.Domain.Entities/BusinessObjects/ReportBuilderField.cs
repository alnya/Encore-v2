namespace Encore.Domain.Entities.BusinessObjects
{
    using System.Collections.Generic;

    public class ReportBuilderField
    {
        public string Id {get; set; }

        public string Name { get; set; }

        public IEnumerable<string> AltNames { get; set; }

        public string Type { get; set; }

        public List<string> SiteIds { get; set; }

        public List<string> Projects { get; set; }
    }
}
