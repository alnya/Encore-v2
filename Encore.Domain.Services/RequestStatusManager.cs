namespace Encore.Domain.Services
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
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

        public void SetStatus(RequestStatus status)
        {
            if (report != null)
            {
                report.LastRequestStatus = status;
                reportRepo.Merge(report.Id, report);
            };

            foreach(var request in requests)
            {
                request.Status = status;
                requestRepo.Merge(request.Id, request);
            }
        }
    }
}
