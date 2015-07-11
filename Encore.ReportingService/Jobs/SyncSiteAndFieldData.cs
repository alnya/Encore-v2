namespace Encore.ReportingService.Jobs
{
    using System.Threading;
    using Domain.Interfaces.Services;
    using Quartz;

    [DisallowConcurrentExecution]
    public class SyncSiteAndFieldData : IJob
    {
        private readonly IProjectService projectService;
        private readonly IFieldService fieldService;

        public SyncSiteAndFieldData(IProjectService projectService, IFieldService fieldService)
        {
            this.projectService = projectService;
            this.fieldService = fieldService;
        }

        public void Execute(IJobExecutionContext context)
        {
            projectService.SyncAllProjectSites();
            projectService.SyncAllProjectFields();

            fieldService.SyncFields();
        }
    }
}
