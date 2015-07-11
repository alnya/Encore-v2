namespace Encore.Domain.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Services.Search;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Extensions;
    using Encore.Domain.Entities.BusinessObjects;

    public class ReportResultService : IReportResultService
    {
        private readonly IRepositoryContext context;

        public ReportResultService(IRepositoryContext context)
        {
            this.context = context;
        }

        public IEnumerable<ReportResultAvailable> GetAvailableResults(Guid userId)
        {
            var requestRepo = context.GetRepository<ReportRequest>();
            var userRequests = requestRepo.GetWhere(x => x.RequestingUserId == userId && x.ResultId.HasValue).OrderByDescending(x => x.RequestDate).Take(5);

            var reportRepo = context.GetRepository<Report>();
            var resultsAvailable = new List<ReportResultAvailable>();

            foreach(var request in userRequests)
            {
                var report = reportRepo.Get(request.ReportId);

                if (report != null)
                {
                    resultsAvailable.Add(new ReportResultAvailable
                    {
                        ReportName = report.Name,
                        ResultId = request.ResultId.Value,
                        RequestDate = request.RequestDate
                    });
                }
            }

            return resultsAvailable;
        }

        public ReportResultsResponse GetResultsPage(Guid resultId, IRequestedPage requestedPage)
        {
            var resultRepo = context.GetRepository<ReportResult>();
            var reportResult = resultRepo.Get(resultId);
            reportResult.ValidateNotNull();

            var reportRepo = context.GetRepository<Report>();
            var report = reportRepo.Get(reportResult.ReportId);
            report.ValidateNotNull();

            var fieldRepo = context.GetRepository<Field>();
            var reportFields = fieldRepo.GetWhere(f => report.FieldIds.Any(id => id == f.SourceId));
            var columns = reportFields.Select(x => new ReportResultColumn { FieldId = x.SourceId, FieldName = x.Name, UnitName = x.Unit });  

            var resultRowRepo = context.GetRepository<ReportResultRow>();
            var results = resultRowRepo.GetWhere(x => x.ReportResultId, resultId, requestedPage);
            var count = resultRowRepo.Count(x => x.ReportResultId, resultId);

            var pagedResults = new PagedListResult<ReportResultRow>(results, count);

            return new ReportResultsResponse
            {
                Count = pagedResults.Count,
                Pages = pagedResults.Pages,
                Rows = pagedResults.Results,
                FieldColumns = columns,
                ReportName = report.Name
            };
        }
        
        public ReportResultsResponse GetAllResults(Guid resultId)
        {
            var resultRepo = context.GetRepository<ReportResult>();
            var reportResult = resultRepo.Get(resultId);
            reportResult.ValidateNotNull();

            var reportRepo = context.GetRepository<Report>();
            var report = reportRepo.Get(reportResult.ReportId);
            report.ValidateNotNull();

            var fieldRepo = context.GetRepository<Field>();
            var reportFields = fieldRepo.GetWhere(f => report.FieldIds.Any(id => id == f.SourceId));
            var columns = reportFields.Select(x => new ReportResultColumn { FieldId = x.SourceId, FieldName = x.Name, UnitName = x.Unit });

            var resultRowRepo = context.GetRepository<ReportResultRow>();
            var rows = resultRowRepo.GetWhere(x => x.ReportResultId, resultId);

            return new ReportResultsResponse
            {
                Count = rows.Count(),
                Pages = 1,
                Rows = rows,
                FieldColumns = columns,
                ReportName = report.Name
            };
        }       
    }
}
