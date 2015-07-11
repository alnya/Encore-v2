namespace Encore.Domain.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using System.Linq;
    using Encore.WebServices;
    using System;
    using Extensions;
    using log4net;
    using System.Collections.Generic;

    public class ProjectService : EntityService<Project>, IProjectService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ProjectService));

        private readonly IRepositoryContext context;

        public ProjectService(IRepositoryContext context)
            : base(context)
        {
            this.context = context;
        }

        public bool SyncProjectSites(Guid projectId)
        {
            log.InfoFormat("Getting sites for project: {0}", projectId);

            var projectRepo = context.GetRepository<Project>();
            var project = projectRepo.Get(projectId);
            project.ValidateNotNull();

            SyncProjectSites(projectRepo, project);

            return true;
        }

        public bool SyncProjectFields(Guid projectId)
        {
            log.InfoFormat("Getting fields for project: {0}", projectId);

            var projectRepo = context.GetRepository<Project>();
            var project = projectRepo.Get(projectId);
            project.ValidateNotNull();

            SyncProjectFields(projectRepo, project);

            return true;
        }
       
        public bool SyncAllProjectSummaries()
        {
            log.Info("Getting all project summary data");
            var projectRepo = context.GetRepository<Project>();
 
            foreach (var project in projectRepo.GetWhere(x => !string.IsNullOrEmpty(x.ApiUrl)))
            {
                // call API and get summary
                var client = new encoreSoapClient("encoreSoap", project.ApiUrl);

                log.InfoFormat("Getting summary data for project: {0}", project.Id);

                var projectSiteSummaries = project.SiteSummaries == null ? new List<ProjectSiteSummary>() : project.SiteSummaries;

                client.BeginGetSummary(
                    result =>
                    {
                        var response = client.EndGetSummary(result);

                        foreach (var item in response)
                        {
                            foreach (var field in item.NumberOfRows)
                            {
                                var currentSummary = projectSiteSummaries.FirstOrDefault(
                                    s => s.FieldSourceId == field.FieldID && s.SiteSourceId == item.SiteID);

                                if (currentSummary != null)
                                {
                                    currentSummary.ValueMaxDate = item.MaxDate;
                                    currentSummary.ValueMinDate = item.MinDate;
                                    currentSummary.RowCount = string.IsNullOrEmpty(field.Value) ? 0 : int.Parse(field.Value);
                                    currentSummary.UpdatedAt = DateTime.Now;
                                }
                                else
                                {
                                    projectSiteSummaries.Add(
                                        new ProjectSiteSummary
                                            {
                                                SiteSourceId = item.SiteID,
                                                FieldSourceId = field.FieldID,
                                                ValueMaxDate = item.MaxDate,
                                                ValueMinDate = item.MinDate,
                                                RowCount = string.IsNullOrEmpty(field.Value) ? 0 : (int.Parse(field.Value)),
                                                CreatedAt = DateTime.Now,
                                                UpdatedAt = DateTime.Now
                                            });
                                }
                            }
                        }

                        projectRepo.Merge(project.Id, new Project
                        {
                            SiteSummaries = projectSiteSummaries,
                            DataLastUpdated = DateTime.Now
                        });

                        log.InfoFormat("Summary data updated for project: {0}", project.Id);
                    },
                    null
                    );
            }

            return true;
        }

        public void SyncAllProjectSites()
        {
            log.Info("Getting all project site data");
            var projectRepo = context.GetRepository<Project>();

            foreach (var project in projectRepo.GetWhere(x => !string.IsNullOrEmpty(x.ApiUrl)))
            {
                SyncProjectSites(projectRepo, project);
            }
        }

        public void SyncAllProjectFields()
        {
            log.Info("Getting all project field data");
            var projectRepo = context.GetRepository<Project>();

            foreach (var project in projectRepo.GetWhere(x => !string.IsNullOrEmpty(x.ApiUrl)))
            {
                SyncProjectFields(projectRepo, project);
            }
        }

        public bool TestProjectUrl(string url)
        {
            var client = new encoreSoapClient("encoreSoap", url);

            try
            {
                var result = client.HelloWorld();
                return result == "Hello World";
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                // Do nothing
            }

            return false;
        }

        private void SyncProjectFields(IRepository<Project> projectRepo, Project project)
        {
            // call API and get summary
            var client = new encoreSoapClient("encoreSoap", project.ApiUrl);

            var projectFields = project.Fields == null ? new List<ProjectField>() : project.Fields;

            client.BeginGetFields(
                result =>
                {
                    var response = client.EndGetFields(result);

                    // sync project sites
                    foreach (var field in response)
                    {
                        if (!projectFields.Any(s => s.SourceId == field.ID))
                        {
                            projectFields.Add(new ProjectField
                            {
                                SourceId = field.ID,
                                Name = field.Name,
                                Unit = field.Unit,
                                CreatedAt = DateTime.Now
                            });
                        }
                    }

                    projectRepo.Merge(project.Id, new Project
                    {
                        Fields = projectFields,
                        DataLastUpdated = DateTime.Now
                    });

                    log.InfoFormat("Fields updated for project: {0}", project.Id);
                },
                null
            );
        }

        private void SyncProjectSites(IRepository<Project> projectRepo, Project project)
        {
            // call API and get summary
            var client = new encoreSoapClient("encoreSoap", project.ApiUrl);

            var projectSites = project.Sites == null ? new List<ProjectSite>() : project.Sites;

            client.BeginGetSites(
                result =>
                {
                    var response = client.EndGetSites(result);

                    // sync project sites
                    foreach (var site in response)
                    {
                        if (!projectSites.Any(s => s.SourceId == site.ID))
                        {
                            projectSites.Add(new ProjectSite
                            {
                                SourceId = site.ID,
                                Name = site.Name,
                                CreatedAt = DateTime.Now
                            });
                        }
                    }

                    projectRepo.Merge(project.Id, new Project
                    {
                        Sites = projectSites,
                        DataLastUpdated = DateTime.Now
                    });

                    log.InfoFormat("Sites updated for project: {0}", project.Id);
                },
                null
            );
        }
    }
}
