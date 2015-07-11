namespace Encore.Web
{
    using System;
    using System.IO;
    using Nancy;

    public class CsvResponse: Response
    {
        public CsvResponse(dynamic model, string filename, ISerializer serializer)
        {
            if (serializer == null)
            {
                throw new InvalidOperationException("CSV Serializer not set");
            }
            
            Contents = GetJsonContents(model, serializer);
            Headers.Add("Content-Disposition", String.Format("attachment; filename={0}.csv", filename));
            ContentType = "text/csv";
            StatusCode = HttpStatusCode.OK;
        }

        private static Action<Stream> GetJsonContents(dynamic model, ISerializer serializer)
        {
            return stream => serializer.Serialize("text/csv", model, stream);
        }
    }
}

