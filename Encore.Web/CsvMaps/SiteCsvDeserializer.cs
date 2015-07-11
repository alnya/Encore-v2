namespace Encore.Web.CsvMaps
{
    using CsvHelper;
    using Encore.Web.CsvMaps;
    using Encore.Web.Models;
    using System.IO;
    
    public class SiteCsvDeserializer : CsvModelBodyDeserializer<Site>
    {
        protected override CsvReader GetCsvReader(TextReader textReader)
        {
            var csvReader = new CsvReader(textReader);
            csvReader.Configuration.RegisterClassMap<SiteCsvClassMap>();

            return csvReader;
        }
    }
}
