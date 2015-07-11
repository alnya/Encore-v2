namespace Encore.Domain.Interfaces.Services
{
    using Encore.Domain.Entities;
    using System;
    
    public interface IAlertReportStatus
    {
        void ReportComplete(Guid resultId, Guid requestingUserId);
    }
}
