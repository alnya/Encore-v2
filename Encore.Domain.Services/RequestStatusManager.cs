namespace Encore.Domain.Services
{
    using System;
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using System.Collections.Generic;
    
    public class RequestStatusManager
    {
        private readonly IRepository<Report> reportRepo;
        private readonly IRepository<ReportRequest> requestRepo;

        private readonly Report report;
        private readonly IEnumerable<ReportRequest> requests;

        public RequestStatusManager(
            IRepository<Report> reportRepo, IRepository<ReportRequest> requestRepo, Report report, IEnumerable<ReportRequest> requests)
        {
            this.reportRepo = reportRepo;
            this.requestRepo = requestRepo;

            this.report = report;
            this.requests = requests;
        }

        public void SetInProgress()
        {
            SetStatus(RequestStatus.InProgress);
        }

        public void SetCompleted(Guid resultId)
        {
            report.LastResultId = resultId;
            SetStatus(RequestStatus.Complete);
        }

        public void SetFailed()
        {
            SetStatus(RequestStatus.Failed);
        }

        private void SetStatus(RequestStatus status)
        {
            report.LastRequestStatus = status;
            reportRepo.Merge(report.Id, report);

            foreach(var request in requests)
            {
                request.Status = status;
                requestRepo.Merge(request.Id, request);
            }
        }
    }
}
