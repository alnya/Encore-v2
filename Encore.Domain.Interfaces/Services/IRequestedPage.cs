using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encore.Domain.Interfaces.Services
{
    public interface IRequestedPage
    {
        int Page { get; }

        int PageSize { get; }
    }
}
