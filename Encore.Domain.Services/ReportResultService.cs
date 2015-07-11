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

        public IEnumerable<ReportResult> GetAvailableResults(Guid userId)
        {
            var resultRepo = context.GetRepository<ReportResult>();
            return resultRepo.GetWhere(x => x.RequestedBy == userId).OrderByDescending(x => x.RunDate).Take(5);
        }

        public ReportResultsResponse GetResultsPage(Guid resultId, IRequestedPage requestedPage)
        {
            var resultRepo = context.GetRepository<ReportResult>();
            var reportResult = resultRepo.Get(resultId);
            reportResult.ValidateNotNull();

            var fieldRepo = context.GetRepository<Field>();
            var reportFields = fieldRepo.GetWhere(f => reportResult.FieldIds.Any(id => id == f.SourceId));
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
                ReportName = reportResult.ReportName
            };
        }
        
        public ReportResultsResponse GetAllResults(Guid resultId)
        {
            var resultRepo = context.GetRepository<ReportResult>();
            var reportResult = resultRepo.Get(resultId);
            reportResult.ValidateNotNull();

            var fieldRepo = context.GetRepository<Field>();
            var reportFields = fieldRepo.GetWhere(f => reportResult.FieldIds.Any(id => id == f.SourceId));
            var columns = reportFields.Select(x => new ReportResultColumn { FieldId = x.SourceId, FieldName = x.Name, UnitName = x.Unit });

            var resultRowRepo = context.GetRepository<ReportResultRow>();
            var rows = resultRowRepo.GetWhere(x => x.ReportResultId, resultId);

            return new ReportResultsResponse
            {
                Count = rows.Count(),
                Pages = 1,
                Rows = rows,
                FieldColumns = columns,
                ReportName = reportResult.ReportName
            };
        }       
    }
}
