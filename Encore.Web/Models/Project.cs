namespace Encore.Web.Models
{
    using Encore.Web.Mapping;
    using System;
    using System.ComponentModel.DataAnnotations;

    [MapsFrom(typeof(Domain.Entities.Project))]
    [MapsTo(typeof(Domain.Entities.Project))]
    public class Project : BaseModel, IHaveCustomMappings
    {
        [Required, StringLength(100, ErrorMessage = "Name cannot be greater than 100 characters.")]
        public String Name { get; set; }

        [StringLength(400, ErrorMessage = "Description cannot be greater than 400 characters.")]
        public string Description { get; set; }

        [StringLength(400, ErrorMessage = "ApiUrl cannot be greater than 400 characters.")]
        public string ApiUrl { get; set; }

        public DateTime? DataLastUpdated { get; set; }

        public int SiteDataCount { get; set; }

        public int FieldDataCount { get; set; }

        public int SummaryDataCount { get; set; }

        public void CreateMappings(AutoMapper.IConfiguration configuration)
        {
            configuration.CreateMap<Domain.Entities.Project, Project>()
                .ForMember(dest => dest.SiteDataCount, opt => opt.MapFrom(src => src.Sites.Count))
                .ForMember(dest => dest.FieldDataCount, opt => opt.MapFrom(src => src.Fields.Count))
                .ForMember(dest => dest.SummaryDataCount, opt => opt.MapFrom(src => src.SiteSummaries.Count));
        }
    }
}
