namespace Encore.ReportingService
{
    using Encore.DataStore;
    using Encore.Domain.Interfaces.DataStore;
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Services;
    using Encore.ReportingService.Jobs;
    using log4net;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Impl.Matchers;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using TinyIoC;
    using Extensions;
    using Encore.PoolParty;

    public class TaskService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(TaskService));

        private MongoContext repositoryContext;
        private PoolPartyClient poolPartyClient;

        private IScheduler scheduler;

        public void Start()
        {
            log.Info("Start requested");

            try
            {
                repositoryContext = new MongoContext(ConfigurationManager.AppSettings["ConnectionString"], "Encore");
                repositoryContext.TryConnect();
                
                poolPartyClient = new PoolPartyClient(
                    ConfigurationManager.AppSettings["PoolParty.EncoreUrl"],
                    ConfigurationManager.AppSettings["PoolParty.UserName"],
                    ConfigurationManager.AppSettings["PoolParty.Password"]);

                var container = new TinyIoCContainer();
                ConfigureContainer(container);

                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                scheduler = schedulerFactory.GetScheduler();

                scheduler.JobFactory = new ServiceJobFactory(container);
                scheduler.ListenerManager.AddJobListener(new LoggingJobListener(), GroupMatcher<JobKey>.AnyGroup());

                scheduler.BuildJob<ProcessReportsJob>().
                    WithRepeatingTrigger(int.Parse(ConfigurationManager.AppSettings["Report.PollIntervalMs"]));

                scheduler.BuildJob<SyncSummaryDataJob>().
                    WithDailyTrigger(TimeOfDay.HourAndMinuteOfDay(0, 0));

                scheduler.BuildJob<CleanUpResultsJob>().
                    UsingJobData("DeleteAfterDays", int.Parse(ConfigurationManager.AppSettings["Report.DeleteAfterDays"])).
                    WithDailyTrigger(TimeOfDay.HourAndMinuteOfDay(1, 0));

                scheduler.BuildJob<SyncSiteAndFieldData>().
                    WithDailyTrigger(TimeOfDay.HourAndMinuteOfDay(2, 0));

                scheduler.Start();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        public void Stop()
        {
            log.Info("Stop requested");
            scheduler.Shutdown();
        }

        private void ConfigureContainer(TinyIoCContainer container)
        {
            container.Register<IRepositoryContext, MongoContext>(repositoryContext);
            container.Register<IProvideFieldData, PoolPartyClient>(poolPartyClient);

            container.Register<IAlertReportStatus, ReportCommunication>();
            container.Register<IProjectService, ProjectService>();
            container.Register<IProcessReportResults, ReportProcessor>();
            container.Register<IFieldService, FieldService>();
        }
    }
}
