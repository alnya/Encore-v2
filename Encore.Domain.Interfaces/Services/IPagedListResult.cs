namespace Encore.Domain.Interfaces.Services
{
    using System.Collections.Generic;

    public interface IPagedListResult<T>
    {
        long Count { get; set; }

        long Pages { get; set; }

        List<T> Results { get; set; }
    }
}
