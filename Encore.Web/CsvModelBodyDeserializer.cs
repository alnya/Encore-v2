namespace Encore.Web
{
    using System;
    using System.Linq;
    using System.IO;
    using CsvHelper;
    using Nancy.ModelBinding;
    using System.Collections.Generic;
    using Encore.Domain.Services.Exceptions;

    public abstract class CsvModelBodyDeserializer<TModelType> : IBodyDeserializer where TModelType : class
    {
        public virtual bool CanDeserialize(string contentType, BindingContext context)
        {
            if (String.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var deserializerIsOfType = context.GenericType == typeof(TModelType);

            var contentMimeType = contentType.Split(';')[0];
            return deserializerIsOfType && contentMimeType.Equals("text/csv", StringComparison.InvariantCultureIgnoreCase);
        }

        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            bodyStream.Position = 0;

            try
            {
                using (TextReader textReader = new StreamReader(bodyStream))
                {
                    using (var csvReader = GetCsvReader(textReader))
                    {
                        return csvReader.GetRecords<TModelType>().ToList();
                    }
                }
            }
            catch (CsvHelperException ex)
            {
                var fieldErrors = new List<string>();

                foreach (var fieldError in ex.Data.Values)
                {
                    fieldErrors.Add(fieldError.ToString());
                }

                throw new DomainException(string.Format(
                    "Invalid CSV format. {0} {1}", ex.Message, String.Join(Environment.NewLine, fieldErrors)));
            }
        }

        protected abstract CsvReader GetCsvReader(TextReader textReader);
    }
}
