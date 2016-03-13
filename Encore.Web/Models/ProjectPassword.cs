namespace Encore.Web.Models
{
    using Encore.Web.Mapping;
    using System;
    using System.ComponentModel.DataAnnotations;

    [MapsFrom(typeof(Domain.Entities.ProjectPassword))]
    [MapsTo(typeof(Domain.Entities.ProjectPassword))]
    public class ProjectPassword
    {
        [Required]
        public Guid ProjectId { get; set; }

        [Required, StringLength(40, ErrorMessage = "Name cannot be greater than 40 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required."), StringLength(128, ErrorMessage = "Password cannot be greater than 128 characters.")]
        public string Password { get; set; }
    }
}
