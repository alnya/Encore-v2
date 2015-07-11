namespace Encore.Domain.Entities.BusinessObjects
{
    using System.Collections.Generic;

    public class ReportBuilderSite
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public List<string> FieldIds { get; set; }

        public List<string> Projects { get; set; }
    }
}
