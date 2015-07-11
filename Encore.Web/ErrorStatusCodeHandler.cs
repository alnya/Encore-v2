namespace Encore.Web
{
    using Encore.Web.Models;
    using Nancy;
    using Nancy.ErrorHandling;
    using Nancy.Responses.Negotiation;
    using Nancy.ViewEngines;
    using System.Linq;

    public sealed class ErrorStatusCodeHandler : IStatusCodeHandler
    {
        private readonly IViewRenderer viewRenderer;
        private readonly IResponseNegotiator negotiator;

        public ErrorStatusCodeHandler(IResponseNegotiator negotiator, IViewRenderer viewRenderer)
        {
            this.viewRenderer = viewRenderer;
            this.negotiator = negotiator;
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound
                   || statusCode == HttpStatusCode.BadRequest
                   || statusCode == HttpStatusCode.InternalServerError
                   || statusCode == HttpStatusCode.Forbidden
                   || statusCode == HttpStatusCode.Unauthorized;
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var error = context.Items.ContainsKey("error") ? context.Items["error"] as Error : null;

            var clientWantsHtml = ShouldRenderFriendlyErrorPage(context);
            if (!clientWantsHtml)
            {
                if (error != null)
                {
                    context.Response = negotiator.NegotiateResponse(error, context).WithStatusCode(error.StatusCode);
                }
                return;
            }

            var errorMessage = error != null ? error.ErrorMessage : string.Empty;

            var response = viewRenderer.RenderView(context, string.Format("errors/{0}", (int)statusCode), new
            {
                Title = statusCode,
                Error = errorMessage
            }).WithStatusCode(statusCode);

            context.Response = response;           
        }

        private static bool ShouldRenderFriendlyErrorPage(NancyContext context)
        {
            var enumerable = context.Request.Headers.Accept;

            var ranges = enumerable.OrderByDescending(o => o.Item2).Select(o => new MediaRange(o.Item1)).ToList();
            foreach (var item in ranges)
            {
                if (item.Matches("text/html"))
                    return true;
                if (item.Matches("application/json"))
                    return false;
                if (item.Matches("application/xml"))
                    return false;
            }

            return true;
        }
    }
}