namespace Encore.ReportingService
{
    using System;
    using Quartz;
    using Quartz.Spi;
    using TinyIoC;

    public class ServiceJobFactory : IJobFactory
    {
        private readonly TinyIoCContainer container;

        public ServiceJobFactory(TinyIoCContainer container)
        {
            this.container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                IJobDetail jobDetail = bundle.JobDetail;
                Type jobType = jobDetail.JobType;

                return (IJob)container.Resolve(jobType);
            }
            catch (Exception ex)
            {
                throw new SchedulerException("Problem instantiating class", ex);
            }
        }

        public void ReturnJob(IJob job)
        {
            // Do nothing - can use this to dispose of job.
        }
    }
}
