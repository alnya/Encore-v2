namespace Encore.Domain.Services.Search
{
    using Encore.Domain.Interfaces.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    [Serializable]
    public class PagedListResult<T> : IPagedListResult<T>
    {
        public PagedListResult() { }

        public PagedListResult(IEnumerable<T> results, long count, int pageSize = 10)
        {
            Results = results.ToList();
            Count = count;

            pageSize = Math.Max(1, pageSize);
            Pages = (count + pageSize - 1) / pageSize;
        }

        public long Count { get; set; }

        public long Pages { get; set; }

        public List<T> Results { get; set; }
    }
}
