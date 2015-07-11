namespace Encore.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy;
    using Nancy.Responses.Negotiation;

    public class CsvResponseProcessor : IResponseProcessor
    {
        private readonly ISerializer serializer;

        public CsvResponseProcessor(IEnumerable<ISerializer> serializers)
        {
            serializer = serializers.FirstOrDefault(x => x.CanSerialize("text/csv"));
        }

        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get { return new[] {new Tuple<string, MediaRange>("csv", new MediaRange("text/csv"))}; }
        }

        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            if (requestedMediaRange.Matches("text/csv") && model is IEnumerable)
            {
                return new ProcessorMatch
                {
                    ModelResult = MatchResult.DontCare,
                    RequestedContentTypeResult = MatchResult.ExactMatch
                };
            }

            return new ProcessorMatch
            {
                ModelResult = MatchResult.DontCare,
                RequestedContentTypeResult = MatchResult.NoMatch
            };
        }

        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            string filename = GetRouteFilename(context.Request);
            return new CsvResponse(model, filename, serializer);
        }

        private string GetRouteFilename(Request request)
        {
            var path = request.Path.TrimEnd('/').Split('/');

            string collectionName;

            if (path.Length > 1 && path.Last().ToLowerInvariant() == "extract")
            {
                collectionName = path[path.Length - 2];
            }
            else
            {
                collectionName = path.LastOrDefault();
            }
            return string.Format("{0}_{1:yyyy-MM-dd_HH-mm-ss}", collectionName ?? "data", DateTime.Now);
        }
    }
}

