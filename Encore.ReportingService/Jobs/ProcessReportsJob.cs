namespace Encore.ReportingService.Jobs
{
    using System.Threading;
    using Domain.Interfaces.Services;
    using Quartz;

    [DisallowConcurrentExecution]
    public class ProcessReportsJob : IJob
    {
        private readonly IProcessReportResults reportProcessor;

        public ProcessReportsJob(IProcessReportResults reportProcessor)
        {
            this.reportProcessor = reportProcessor;
        }

        public void Execute(IJobExecutionContext context)
        {
            reportProcessor.ProcessReportQueue();
        }
    }
}
