namespace Encore.Web
{
    using System;
    using log4net.Config;
    using log4net;
    using Topshelf;

    class Program
    {
        public static void Main()
        {
            XmlConfigurator.Configure();
            var log = LogManager.GetLogger(typeof(Program));
            log.Info("Logging configured");

            HostFactory.Run(x =>
            {
                x.Service<ApplicationService>(s =>
                {
                    s.ConstructUsing(name => new ApplicationService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                x.UseLog4Net();
                x.StartAutomatically();
                x.RunAsLocalSystem();
                x.SetDescription("Service for running Encore Web Application");
                x.SetDisplayName("Encore Application Service");
                x.SetServiceName("EncoreApplicationService");
            });
        }
    }
}
