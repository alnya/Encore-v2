namespace Encore.Web
{
    using log4net;
    using Nancy.Hosting.Self;
    using System;
    using System.Configuration;

    public class ApplicationService
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ApplicationService));
        private NancyHost host;
        
        public void Start()
        {
            log.Info("Start requested");

            var uri = new Uri(ConfigurationManager.AppSettings["ServiceUrl"] ?? "http://+:8080");

            host = new NancyHost(uri);
            host.Start();
            log.Info("Application is running on " + uri);
        }

        public void Stop()
        {
            log.Info("Stop requested");

            if (host != null)
            {
                host.Dispose();
            }
        }
    }
}
