namespace Encore.Domain.Interfaces.Services
{
    using Encore.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    public interface ISiteService
    {
        IPagedListResult<Site> Search(IRequestedPage requestedPage, ISortCriteria sortCriteria = null, ISearchTerms searchTerms = null);

        bool ReplaceSites(ICollection<Site> sites);

        bool DeleteAll();
    }
}
