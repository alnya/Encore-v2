namespace Encore.Web.CsvMaps
{
    using CsvHelper.Configuration;
    using Encore.Web.Models;

    public sealed class SiteCsvClassMap : CsvClassMap<Site>
    {
        public SiteCsvClassMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.Type).Name("Type");
        }
    }
}
