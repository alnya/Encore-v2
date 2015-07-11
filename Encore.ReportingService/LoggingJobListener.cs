namespace Encore.ReportingService
{
    using log4net;
    using Quartz;
    using Quartz.Impl;

    public class LoggingJobListener : IJobListener
    {
        private readonly ILog log = LogManager.GetLogger(typeof(LoggingJobListener));

        public void JobExecutionVetoed(IJobExecutionContext context)
        {
            // Do nothing
        }

        public void JobToBeExecuted(IJobExecutionContext context)
        {
            // Do nothing
        }

        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            var jobName = ((JobDetailImpl)context.JobDetail).Name;

            if (jobException != null)
            {
                log.Error(string.Format("Job {0} error", jobName), jobException);
            }
            else
            {
                log.Debug(string.Format("Job {0} complete", jobName));
            }
        }

        public string Name
        {
            get { return "LoggingJobListener"; }
        }
    }
}
