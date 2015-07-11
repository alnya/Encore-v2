namespace Encore.ReportingService.Extensions
{
    using Quartz;
    using System;
    
    public static class SchedulerExtensions
    {
        public class SchedulerJobConfig
        {
            public IScheduler Scheduler { get; private set; }

            public IJobDetail JobDetail { get; private set; }

            public SchedulerJobConfig(IScheduler scheduler, IJobDetail jobDetail)
            {
                Scheduler = scheduler;
                JobDetail = jobDetail;
            }
        }

        public static SchedulerJobConfig BuildJob<T>(this IScheduler scheduler, string name = null, string group = "Encore") where T : IJob
        {
            if (name == null)
            {
                name = typeof(T).Name;
            }

            var job = JobBuilder.Create<T>().WithIdentity(name, group).Build();
            return new SchedulerJobConfig(scheduler, job);
        }

        public static SchedulerJobConfig UsingJobData(this SchedulerJobConfig schedulerJobConfig, string key, object value)
        {
            schedulerJobConfig.JobDetail.JobDataMap.Add(key, value);
            return schedulerJobConfig;
        }

        public static IScheduler WithRepeatingTrigger(this SchedulerJobConfig schedulerJobConfig, int intervalMs)
        {
            // Repeat task forever, throwing away misfired tasks
            var trigger = TriggerBuilder.Create()
                .WithSimpleSchedule(builder => builder
                    .WithIntervalInSeconds(intervalMs / 1000)
                    .RepeatForever()
                    .WithMisfireHandlingInstructionNextWithRemainingCount())
                .Build();

            schedulerJobConfig.Scheduler.ScheduleJob(schedulerJobConfig.JobDetail, trigger);
            return schedulerJobConfig.Scheduler;
        }

        public static IScheduler WithDailyTrigger(this SchedulerJobConfig schedulerJobConfig, TimeOfDay timeOfDay)
        {
            // Repeat task every day at specified time. 
            // Misfires are rolled togeter and executed at next available opportunity.
            var trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(builder =>
                    builder.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(timeOfDay)
                        .WithMisfireHandlingInstructionFireAndProceed())
                .Build();

            schedulerJobConfig.Scheduler.ScheduleJob(schedulerJobConfig.JobDetail, trigger);
            return schedulerJobConfig.Scheduler;
        }
    }
}
