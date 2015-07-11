namespace Encore.Domain.Interfaces.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Entities.BusinessObjects;
    using System;
    using System.Collections.Generic;
    
    public interface IReportResultService
    {
        ReportResultsResponse GetResultsPage(Guid resultId, IRequestedPage requestedPage);

        ReportResultsResponse GetAllResults(Guid resultId);

        IEnumerable<ReportResult> GetAvailableResults(Guid userId);
    }
}
