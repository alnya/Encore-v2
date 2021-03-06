﻿namespace Encore.Domain.Interfaces.Services
{
    using Encore.Domain.Entities;
    using System.Collections.Generic;
    
    public interface ISiteService
    {
        IPagedListResult<Site> Search(IRequestedPage requestedPage, ISortCriteria sortCriteria = null, ISearchTerms searchTerms = null);

        bool ReplaceSites(ICollection<Site> sites);

        bool DeleteAll();
    }
}
