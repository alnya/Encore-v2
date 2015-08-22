namespace Encore.Domain.Services
{
    using System;
    using System.Linq;
    using System.Text;
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using log4net;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Extensions;

    public class ReportProcessor : IProcessReportResults
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ReportProcessor));

        private readonly IAlertReportStatus reportAlerts;
        private readonly IRepository<ReportRequest> reportRequestRepo;
        private readonly IRepository<Report> reportRepo;
        private readonly IRepository<Project> projectRepo;
        private readonly IRepository<Site> siteRepo;
        private readonly IRepository<Field> fieldRepo;
        private readonly IRepository<ReportResult> resultRepo;
        private readonly IRepository<ReportResultRow> resultRowRepo;

        public ReportProcessor(IRepositoryContext context, IAlertReportStatus reportAlerts)
        {
            this.reportAlerts = reportAlerts;

            this.reportRequestRepo = context.GetRepository<ReportRequest>();
            this.reportRepo = context.GetRepository<Report>();
            this.projectRepo = context.GetRepository<Project>();
            this.siteRepo = context.GetRepository<Site>();
            this.fieldRepo = context.GetRepository<Field>();
            this.resultRepo = context.GetRepository<ReportResult>();
            this.resultRowRepo = context.GetRepository<ReportResultRow>();
        }       

        public void ProcessReportQueue()
        {
            var pendingTasks = reportRequestRepo.GetWhere(r => r.Status == RequestStatus.Pending).ToList();

            if(!pendingTasks.Any())
                return;

            var projects = projectRepo.GetWhere();
            var sites = siteRepo.GetWhere();
            var fields = fieldRepo.GetWhere();

            var reportRequests = pendingTasks.GroupBy(x=> x.ReportId).ToDictionary(x => x.Key, x => x);

            foreach (var reportRequest in reportRequests)
            {
                var report = reportRepo.Get(reportRequest.Key);
                var requests = reportRequest.Value;

                var statusManager = new RequestStatusManager(
                    reportRepo, 
                    reportRequestRepo, 
                    report,
                    requests);

                var reportResult = RunRequests(resultRepo, report, projects, sites, fields, statusManager);

                if (reportResult != null)
                {
                    reportAlerts.ReportComplete(reportResult.Id, report.CreatedBy);
                }
            }
        }

        private ReportResult RunRequests(
            IRepository<ReportResult> resultRepo,
            Report report,
            IEnumerable<Project> projects,
            IEnumerable<Site> sites,
            IEnumerable<Field> fields,
            RequestStatusManager statusManager)
        {
            if (report == null)
            {
                statusManager.SetFailed();
                return null;
            }

            log.InfoFormat("Getting data for report {0}", report.Id);

            var reportFields = fields.Where(f => report.FieldIds.Any(id => id == f.SourceId));
            var reportSites = sites.Where(s => report.SiteIds.Any(id => id == s.Id));

            statusManager.SetInProgress();

            try
            {
                var projectTasks = new List<Task<IEnumerable<ReportResultRow>>>();

                var reportResult = new ReportResult
                {
                    ReportId = report.Id,
                    ReportName = report.Name,
                    FieldIds = report.FieldIds,
                    RequestedBy = report.CreatedBy,
                    RunDate = DateTime.Now
                };

                resultRepo.Save(reportResult);

                foreach (var project in projects)
                {
                    projectTasks.Add(Task.Factory.StartNew(() => GenerateReportRows(reportResult.Id, report, project, reportSites, reportFields)));
                }

                Task.WaitAll(projectTasks.ToArray());

                var reportResults = new List<ReportResultRow>();

                foreach (var task in projectTasks)
                {
                    reportResults.AddRange(task.Result);
                }

                resultRowRepo.Insert(reportResults.OrderByDescending(x => x.RowDateTime));

                statusManager.SetCompleted(reportResult.Id);
                return reportResult;
            }
            catch (Exception ex)
            {
                statusManager.SetFailed();
                log.Error(string.Format("Get Data for report {0} failed", report.Id), ex);
                return null;
            }
        }

        public void RemoveOldResults(int deleteAfterDays)
        {
            var deleteResults = resultRepo.GetWhere(r => r.RunDate.AddDays(deleteAfterDays) < DateTime.Now);
            log.InfoFormat("Found {0} sets of results to remove", deleteResults.Count());

            foreach (var deleteResult in deleteResults)
            {
                log.InfoFormat("Removing results for report {0} requested on {1}", deleteResult.ReportId, deleteResult.RunDate);
                resultRowRepo.DeleteWhere(x => x.ReportResultId == deleteResult.Id);
                resultRepo.DeleteWhere(x => x.Id == deleteResult.Id);
            }
        }

        private IEnumerable<ReportResultRow> GenerateReportRows(
            Guid resultId, Report report, Project project, IEnumerable<Site> reportSites, IEnumerable<Field> reportFields)
        {
            var projectFields = project.Fields.Where(pf => reportFields.Any(rf => rf.ProjectIds.Any(id => id == project.FieldPrefix + pf.SourceId)));
            var projectSites = project.Sites.Where(ps => reportSites.Any(rs => String.Equals(rs.Name, ps.Name, StringComparison.OrdinalIgnoreCase)));

            // call API and get data
            var client = new Encore.WebServices.encoreSoapClient("encoreSoap", project.ApiUrl);

            var projectFieldSourceIds = projectFields.Select(f => f.SourceId).ToList();
            var projectSiteSourceIds = projectSites.Select(s => s.SourceId).ToList();

            if (projectFieldSourceIds.Any() && projectSiteSourceIds.Any())
            {    
                log.InfoFormat("Asking project {0} for report data", project.Id);

                var response = client.GetData(
                    projectFieldSourceIds.ToArray(),
                    projectSiteSourceIds.ToArray(),
                    report.DateFrom,
                    report.DateTo);

                if (response.Rows != null)
                {
                    log.InfoFormat("Received {0} rows of data", response.Rows.Count());
                    return GenerateReportRows(resultId, reportFields.ToSourceIdMap(project.FieldPrefix), projectSites.ToProjectIdMap(), response.Rows);
                }
            }

            return new List<ReportResultRow>();
        }

        private IEnumerable<ReportResultRow> GenerateReportRows(
            Guid resultId, Dictionary<int, Field> fieldMap, Dictionary<int, String> siteMap, WebServices.DataPackageRow[] responseRows)
        {
            var reportRows = new List<ReportResultRow>();

            foreach(var srcRow in responseRows)
            {
                var reportRow = new ReportResultRow
                {
                    ReportResultId = resultId,
                    RowDateTime = srcRow.RowDate,
                    SiteId = srcRow.SiteID
                };

                if (siteMap.ContainsKey(srcRow.SiteID))
                {
                    reportRow.SiteName = siteMap[srcRow.SiteID];
                }

                foreach (var rowValue in srcRow.Values)
                {
                    if (fieldMap.ContainsKey(rowValue.FieldID))
                    {
                        reportRow.Fields.Add(new ReportResultField
                            {
                                FieldId = fieldMap[rowValue.FieldID].SourceId,
                                ProjectId = rowValue.FieldID,
                                Value = rowValue.Value
                            });
                    }
                }

                reportRows.Add(reportRow);
            }

            return reportRows;
        }
    }
}
