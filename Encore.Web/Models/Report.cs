namespace Encore.Web.Models
{
    using Encore.Domain.Entities;
    using Encore.Web.Mapping;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MapsFrom(typeof(Domain.Entities.Report))]
    [MapsTo(typeof(Domain.Entities.Report))]
    public class Report : BaseModel
    {
        [Required, StringLength(100, ErrorMessage = "Name cannot be greater than 100 characters.")]
        public String Name { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public DateTime LastRequested { get; set; }

        public String LastRequestStatus { get; set; }

        public IEnumerable<string> FieldIds;

        public IEnumerable<string> SiteIds;
    }
}
