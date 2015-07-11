namespace Encore.Domain.Interfaces.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Entities.BusinessObjects;
    using System;
    using System.Collections.Generic;

    public interface IReportService : IEntityService<Report> 
    { 
        ReportBuilderData GetBuilderData();

        ReporDataSummary GetReportDataSummary(DateTime fromDate, DateTime toDate, IEnumerable<string> siteIds, IEnumerable<string> fieldIds);

        bool RequestReport(Guid reportId);
    }
}
