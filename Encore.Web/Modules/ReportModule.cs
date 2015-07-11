namespace Encore.Web.Modules
{
    using AutoMapper;
    using Encore.Domain.Interfaces.Services;
    using Nancy;
    using Nancy.Security;
    using System;
    using System.Linq;
    using Extensions;
    using Encore.Domain.Entities;
    using System.Collections.Generic;
    using Encore.Domain.Entities.BusinessObjects;
    using Nancy.ModelBinding;

    public class ReportModule : SecureModule
    {
        private const string FieldNameFormat = "{0} {1}";
        private const string RowDateFormat = "yyyy/MM/dd HH:mm";

        private readonly IReportService reportService;

        private readonly IReportResultService reportResultService;

        public ReportModule(IReportService reportService, IReportResultService reportResultService, IMappingEngine mappingEngine)
            : base("data/reports", mappingEngine)
        {
            this.reportService = reportService;
            this.reportResultService = reportResultService;

            Get["/"] = SearchReports;
            
            Get["/{id}"] = GetReport;

            Put["/{id}/request"] = RequestReport;

            Post["/"] = AddReport;

            Get["/myResults"] = GetMyResults;

            Get["/results/{id}"] = ViewReportResults;

            Get["/results/{id}/download/{*}"] = DownloadReportResults;

            Get["/builder"] = GetReportBuilderData;

            Get["/builder/summary"] = GetReportDataSummary; 
        }

        private dynamic SearchReports(dynamic args)
        {
            var entities = reportService.Search(
                Context.RequestedPage(),
                x => x.CreatedBy == AuthorizedUserId,
                Context.SortCriteria<Report>(),
                Context.SearchTerms<Report>());

            return MapToResultList<Report, Encore.Web.Models.Report>(entities);
        }

        private dynamic GetReport(dynamic args)
        {
            var report = reportService.Get(new Guid(args.id));

            if (report == null)
            {
                return Negotiate.WithStatusCode(HttpStatusCode.NotFound);
            }

            if (report.CreatedBy != AuthorizedUserId)
            {
                return Negotiate.WithStatusCode(HttpStatusCode.Unauthorized);
            }

            return MapTo<Encore.Web.Models.Report>(report);
        }

        private dynamic AddReport(dynamic args)
        {
            var model = this.BindAndValidate<Encore.Web.Models.Report>();

            if (!ModelValidationResult.IsValid)
            {
                return RespondWithValidationFailure(ModelValidationResult);
            }

            var addEntity = MapTo<Report>(model);
            addEntity.CreatedBy = AuthorizedUserId;
            var returnEntity = reportService.Add(addEntity);

            reportService.RequestReport(returnEntity.Id);
            return MapTo<Encore.Web.Models.Report>(returnEntity);
        }

        private dynamic RequestReport(dynamic args)
        {
            var report = reportService.Get(new Guid(args.id));

            if (report == null)
            {
                return Negotiate.WithStatusCode(HttpStatusCode.NotFound);
            }

            if (report.CreatedBy != AuthorizedUserId)
            {
                return Negotiate.WithStatusCode(HttpStatusCode.Unauthorized);
            }

            return reportService.RequestReport(report.Id);
        }

        private dynamic GetMyResults(dynamic args)
        {
            var userResults = reportResultService.GetAvailableResults(AuthorizedUserId);

            return MapTo<IEnumerable<Encore.Web.Models.ReportResultAvailable>>(userResults);
        }

        private dynamic ViewReportResults(dynamic args)
        {
            var requestId = new Guid(args.id);

            var results = reportResultService.GetResultsPage(requestId, Context.RequestedPage());

            var columns = results.FieldColumns.Select(x => new { name = String.Format(FieldNameFormat, x.FieldName, x.UnitName), id = x.FieldId }).ToList();
            columns.Insert(0, new { name = "Date Recorded", id = "DateRecorded" });
            columns.Insert(1, new { name = "Site Name", id = "SiteName" });

            var pagedResult = new
            {
                ReportName = results.ReportName,
                Count = results.Count,
                Pages = results.Pages,
                Columns = columns,
                Rows = results.Rows.Select(x =>
                {
                    var row = new DynamicDictionary();
                    row.Add("DateRecorded", x.RowDateTime.ToString(RowDateFormat));
                    row.Add("SiteName", x.SiteName);

                    foreach (var field in x.Fields)
                    {
                        row.Add(field.FieldId, field.Value);
                    }

                    return row;
                })
            };

            return pagedResult;
        }

        private dynamic DownloadReportResults(dynamic args)
        {
            var requestId = new Guid(args.id);

            var allResults = reportResultService.GetAllResults(requestId);
            var colNameLookup = allResults.FieldColumns.ToDictionary(x => x.FieldId, x => String.Format(FieldNameFormat, x.FieldName, x.UnitName));

            return allResults.Rows.Select(x =>
            {
                var row = new DynamicDictionary();
                row.Add("Date Recorded", x.RowDateTime.ToString(RowDateFormat));
                row.Add("Site Name", x.SiteName);

                foreach (var field in x.Fields)
                {
                    row.Add(colNameLookup[field.FieldId], field.Value);
                }

                return row;
            });          
        }
        
        private dynamic GetReportBuilderData(dynamic args)
        {
            var reportBuilderData = reportService.GetBuilderData();

            return MapTo<Encore.Web.Models.ReportBuilderData>(reportBuilderData);
        }

        private dynamic GetReportDataSummary(dynamic args)
        {
            if (Context.Request.Query.SelectedSiteIds.HasValue &&
                Context.Request.Query.SelectedFieldIds.HasValue &&
                Context.Request.Query.FromDate.HasValue &&
                Context.Request.Query.ToDate.HasValue)
            {
                var siteIds = Context.Request.Query.SelectedSiteIds.ToString().Split(',');
                var fieldIds = Context.Request.Query.SelectedFieldIds.ToString().Split(',');

                DateTime fromDate;
                DateTime toDate;

                var fromValid = DateTime.TryParse(Context.Request.Query.FromDate, out fromDate);
                var toValid = DateTime.TryParse(Context.Request.Query.ToDate, out toDate);

                if (fromValid && toValid)
                {
                    var reportSummary = reportService.GetReportDataSummary(fromDate, toDate, siteIds, fieldIds);

                    if (reportSummary != null)
                    {
                        return MapTo<Encore.Web.Models.ReportDataSummary>(reportSummary);
                    }
                }
            }

            return new Encore.Web.Models.ReportDataSummary
            {
                rows = 0
            };
        }
    }
}
