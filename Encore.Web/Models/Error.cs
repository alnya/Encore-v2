namespace Encore.Web.Models
{
    using Nancy;

    public class Error
    {
        public string ErrorMessage { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}
