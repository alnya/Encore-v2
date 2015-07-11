namespace Encore.ReportingService.Jobs
{
    using System.Threading;
    using Domain.Interfaces.Services;
    using Quartz;

    [DisallowConcurrentExecution]
    public class CleanUpResultsJob : IJob
    {
        private readonly IProcessReportResults reportProcessor;

        public CleanUpResultsJob(IProcessReportResults reportProcessor)
        {
            this.reportProcessor = reportProcessor;
        }

        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            reportProcessor.RemoveOldResults(dataMap.GetInt("DeleteAfterDays"));
        }
    }
}
