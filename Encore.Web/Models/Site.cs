namespace Encore.Web.Models
{
    using Encore.Web.Mapping;
    using System.ComponentModel.DataAnnotations;

    [MapsFrom(typeof(Domain.Entities.Site))]
    [MapsTo(typeof(Domain.Entities.Site))]
    public class Site : BaseModel
    {
        [Required, StringLength(100, ErrorMessage = "Name cannot be greater than 100 characters.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Type cannot be greater than 100 characters.")]
        public string Type { get; set; }
    }
}
