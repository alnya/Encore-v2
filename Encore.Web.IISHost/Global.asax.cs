namespace Encore.Web.IISHost
{
    using log4net;
    using log4net.Config;
    using Encore.Web;

    public class Global : System.Web.HttpApplication
    {
        private readonly ILog log = LogManager.GetLogger(typeof(Global));

        public void Application_Start()
        {
            XmlConfigurator.Configure();
            log.Info("Logging configured");

            Bootstrapper.RootDirectory = Server.MapPath("~/bin");
        }
    }
}