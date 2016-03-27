namespace Encore.Web.Models
{
    using Encore.Web.Mapping;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MapsFrom(typeof(Domain.Entities.SystemUser))]
    [MapsTo(typeof(Domain.Entities.SystemUser))]
    public class User : BaseModel
    {
        [Required, StringLength(40, ErrorMessage = "Name cannot be greater than 40 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required."), StringLength(128, ErrorMessage = "Password cannot be greater than 128 characters.")]
        public string Password { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot be greater than 100 characters.")]
        public string Email { get; set; }

        public string UserRole { get; set; }

        public List<ProjectToken> ProjectTokens { get; set; }
    }
}
