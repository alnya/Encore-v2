
namespace Encore.Web.IISHost
{
    using Owin;
    using Microsoft.Owin.Extensions;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy();

            app.UseStageMarker(PipelineStage.MapHandler); // Required so that static files can be served up under IIS hosting.
        }
    }
}
