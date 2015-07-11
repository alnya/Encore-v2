namespace Encore.ReportingService
{
    using log4net;
    using log4net.Config;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Topshelf;
    
    static class Program
    {
        public static void Main()
        {
            XmlConfigurator.Configure();
            var log = LogManager.GetLogger(typeof(Program));
            log.Info("Logging configured");

            HostFactory.Run(x =>
            {
                x.Service<TaskService>(s =>
                {
                    s.ConstructUsing(name => new TaskService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                x.UseLog4Net();
                x.StartAutomatically();
                x.RunAsLocalSystem();
                x.SetDescription("Service for running Encore Reporting Tasks");
                x.SetDisplayName("Encore Task Service");
                x.SetServiceName("EncoreTaskService");
            });
        }
    }
}
