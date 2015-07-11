namespace Encore.Domain.Services.Search
{
    using Encore.Domain.Interfaces.Services;
    using System;
    
    public class RequestedPage : IRequestedPage
    {
        public static RequestedPage Default()
        {
            return new RequestedPage { Page = 1, PageSize = 10 };
        }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}
