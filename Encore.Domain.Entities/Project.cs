namespace Encore.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Project : EntityBase
    {
        public string Name {get; set; }

        public string Description { get; set; }

        public string FieldPrefix { get; set; }

        public string ApiUrl { get; set; }

        public DateTime? DataLastUpdated { get; set; }

        public List<ProjectSite> Sites { get; set; }

        public List<ProjectField> Fields { get; set; }

        public List<ProjectSiteSummary> SiteSummaries { get; set; }
    }
}
