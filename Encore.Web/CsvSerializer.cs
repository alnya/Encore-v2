namespace Encore.Web
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using CsvHelper;
    using Nancy;
    using Nancy.IO;

    public class CsvSerializer : ISerializer
    {
        public bool CanSerialize(string contentType)
        {
            return IsCsvType(contentType);
        }

        public IEnumerable<string> Extensions
        {
            get { yield return "csv"; }
        }

        public void Serialize<TModel>(string contentType, TModel model, Stream outputStream)
        {
            var enumerable = model as IEnumerable;

            if (enumerable == null)
                return;

            using (var streamWriter = new StreamWriter(new UnclosableStreamWrapper(outputStream)))
            {
                using (var csvWriter = GetCsvWriter(streamWriter))
                {
                    bool isFirst = true;

                    foreach (var record in enumerable)
                    {
                        if (isFirst)
                        {
                            WriteHeader(csvWriter, record);
                            isFirst = false;
                        }

                        WriteRecord(csvWriter, record);
                    }
                }
            }
        }

        private void WriteHeader(CsvWriter csvWriter, dynamic record)
        {
            var dynamicRecord = record as IDictionary<string, object>;

            if (dynamicRecord == null)
            {
                csvWriter.WriteHeader(record.GetType());
            }
            else
            {
                // Handle ExpandoObject or other property value map.
                // CsvWriter won't accept a record that implements IEnumerable.
                foreach (var propertyName in dynamicRecord.Keys)
                {
                    csvWriter.WriteField(propertyName);
                }

                csvWriter.NextRecord();
            }
        }

        private void WriteRecord(CsvWriter csvWriter, dynamic record)
        {
            var dynamicRecord = record as IDictionary<string, object>;

            if (dynamicRecord == null)
            {
                csvWriter.WriteRecord(record);
            }
            else
            {
                // Handle ExpandoObject or other property value map.
                // CsvWriter won't accept a record that implements IEnumerable.
                foreach (var propertyValue in dynamicRecord.Values)
                {
                    csvWriter.WriteField(propertyValue);
                }

                csvWriter.NextRecord();
            }
        }

        private static CsvWriter GetCsvWriter(StreamWriter streamWriter)
        {
            var csv = new CsvWriter(streamWriter);
            csv.Configuration.QuoteAllFields = true;
            return csv;
        }

        private static bool IsCsvType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("text/csv", StringComparison.InvariantCultureIgnoreCase) ||
                   contentMimeType.StartsWith("text/csv", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
