namespace Encore.Domain.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Entities.BusinessObjects;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;

    public class ReportService : EntityService<Report>, IReportService
    {
        private readonly IRepositoryContext context;

        public ReportService(IRepositoryContext context)
            : base(context)
        {
            this.context = context;
        }

        public bool RequestReport(Guid reportId)
        {
            var reportRepo = context.GetRepository<Report>();
            var report = reportRepo.Get(reportId);
            report.ValidateNotNull();

            var requestRepo = context.GetRepository<ReportRequest>();

            var request = new ReportRequest
            {
                ReportId = reportId,
                RequestingUserId = report.CreatedBy,
                RequestDate = DateTime.Now,
                Status = RequestStatus.Pending
            };

            requestRepo.Save(request);

            report.LastRequested = request.RequestDate;
            report.LastRequestStatus = RequestStatus.Pending;

            reportRepo.Merge(reportId, report);

            return true;
        }

        public ReportBuilderData GetBuilderData()
        {
            var siteRepo = context.GetRepository<Site>();            
            var fieldRepo = context.GetRepository<Field>();
            var projectRepo = context.GetRepository<Project>();

            var sites = siteRepo.GetWhere();
            var fields = fieldRepo.GetWhere();
            var projects = projectRepo.GetWhere(p => p.SiteSummaries != null && p.SiteSummaries.Any(s => s.RowCount > 0));

            var builderData = new ReportBuilderData
            {
                SiteTypes = sites.Select(x => x.Type).Distinct().ToList(),
                FieldTypes = fields.Select(x => x.Type).Distinct().ToList(),
                Projects = projects.Select(x => x.Name).ToList(),
                SiteData = new List<ReportBuilderSite>(),
                FieldData = new List<ReportBuilderField>()
            };

            var projectSiteIdMap = new Dictionary<int, Guid>();

            // Map of Project SiteId to Global Site
            foreach (var site in sites)
            {
                foreach (var siteId in projects.SelectMany(p => p.Sites.Where(s => site.Name.Equals(s.Name, StringComparison.OrdinalIgnoreCase)).Select(s => s.SourceId)))
                {
                    if (!projectSiteIdMap.ContainsKey(siteId))
                    {
                        projectSiteIdMap.Add(siteId, site.Id);
                    }
                }
            }

            // Get site data
            var siteData = new List<ReportBuilderSite>();
                                 
            foreach (var site in sites)
            {
                var reportSite = new ReportBuilderSite
                {
                    Id = site.Id.ToString(),
                    Name = site.Name,
                    Type = site.Type,
                    Projects = new List<string>(),
                    FieldIds = new List<string>()
                };

                var inProjects = projects.Where(p => p.SiteSummaries.Any(summary => SiteFoundInSummary(projectSiteIdMap, site, summary))).ToList();

                if (inProjects.Any())
                {
                    reportSite.Projects = inProjects.Select(x => x.Name).ToList();                   

                    // Get all project field Ids present in summary data for this site, map to global Field Ids
                    var projectFieldIds = inProjects.SelectMany(p => p.SiteSummaries.Where(summary => SiteFoundInSummary(projectSiteIdMap, site, summary)).Select(s => s.FieldSourceId)).Distinct().ToList();
                    reportSite.FieldIds = fields.Where(f => f.ProjectIds.Any(id => projectFieldIds.Any(x => x == id))).Select(f => f.SourceId).ToList();

                    siteData.Add(reportSite);
                }
            };

            builderData.SiteData.AddRange(siteData.OrderBy(x => x.Name));
            
            // Get field data
            var fieldData = new List<ReportBuilderField>();

            foreach (var field in fields)
            {
                var reportField = new ReportBuilderField
                {
                    Id = field.SourceId,
                    Name = field.Name,
                    Type = field.Type,
                    Projects = new List<string>(),
                    SiteIds = new List<string>()
                };

                var inProjects = projects.Where(p => p.SiteSummaries.Any(s => field.ProjectIds.Any(fid => s.FieldSourceId == fid))).ToList();

                if (inProjects.Any())
                {
                    reportField.Projects = inProjects.Select(x => x.Name).ToList();

                    // Get all project site Ids present in summary data for this site, map to global Site Ids
                    var projectSiteIds = inProjects.
                        SelectMany(p => p.SiteSummaries.Where(x => field.ProjectIds.Any(fid => x.FieldSourceId == fid))).Select(s => s.SiteSourceId).
                        Distinct().ToList();

                    reportField.SiteIds = projectSiteIds.Where(x => projectSiteIdMap.ContainsKey(x)).Select(x => projectSiteIdMap[x].ToString()).ToList();

                    fieldData.Add(reportField);
                }
            };

            builderData.FieldData.AddRange(fieldData.OrderBy(x => x.Name));

            return builderData;
        }

        public ReporDataSummary GetReportDataSummary(DateTime fromDate, DateTime toDate, IEnumerable<string> siteIds, IEnumerable<string> fieldIds)
        {
            var siteRepo = context.GetRepository<Site>();
            var fieldRepo = context.GetRepository<Field>();
            var projectRepo = context.GetRepository<Project>();

            var projects = projectRepo.GetWhere();
            var fields = fieldRepo.GetWhere(x => fieldIds.Any(id => id == x.SourceId));
            var projectFieldIds = fields.SelectMany(x => x.ProjectIds).ToList();

            var siteNames = siteRepo.GetWhere(x => siteIds.Any(id => new Guid(id) == x.Id)).Select(x => x.Name);
            var projectSiteIds = projects.SelectMany(p => p.Sites.Where(s => siteNames.Any(name => String.Equals(name, s.Name, StringComparison.OrdinalIgnoreCase))).Select(x => x.SourceId).ToList());

            var projectRows = projects.SelectMany(p => p.SiteSummaries.Where(s =>
                s.ValueMinDate >= fromDate &&
                s.ValueMaxDate <= toDate &&
                projectSiteIds.Any(id => id == s.SiteSourceId) &&
                projectFieldIds.Any(id => id == s.FieldSourceId))).ToList();

            if (projectRows.Count == 0)
            {
                return null;
            }

            return new ReporDataSummary
            {
                DateFrom = projectRows.Min(x => x.ValueMinDate),
                DateTo = projectRows.Max(x => x.ValueMaxDate),
                Rows = projectRows.Sum(x => x.RowCount)
            };
        }

        private static bool SiteFoundInSummary(Dictionary<int, Guid> projectSiteIdMap, Site site, ProjectSiteSummary siteSummary)
        {
            return projectSiteIdMap.ContainsKey(siteSummary.SiteSourceId) && projectSiteIdMap[siteSummary.SiteSourceId] == site.Id;
        }
    }
}
