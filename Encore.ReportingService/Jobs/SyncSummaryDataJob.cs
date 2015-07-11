namespace Encore.ReportingService.Jobs
{
    using System.Threading;
    using Domain.Interfaces.Services;
    using Quartz;

    [DisallowConcurrentExecution]
    public class SyncSummaryDataJob : IJob
    {
        private readonly IProjectService projectService;

        public SyncSummaryDataJob(IProjectService projectService)
        {
            this.projectService = projectService;
        }

        public void Execute(IJobExecutionContext context)
        {
            projectService.SyncAllProjectSummaries();
        }
    }
}
