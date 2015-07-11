namespace Encore.Web.Extensions
{
    using Encore.Domain.Entities;
    using Encore.Domain.Interfaces.Services;
    using Encore.Domain.Services.Search;
    using Encore.Web.Security;
    using Nancy;
    using Nancy.Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    public static class NancyContextExtensions
    {
        private const string IdentityKey = "Identity";

        public static IRequestedPage RequestedPage(this NancyContext context)
        {
            return new RequestedPage
            {
                Page = context.Request.Query.Page.HasValue ? int.Parse(context.Request.Query.Page) : 1,
                PageSize = context.Request.Query.Pagesize.HasValue ? int.Parse(context.Request.Query.Pagesize) : 10
            };
        }

        public static ISortCriteria SortCriteria<T>(this NancyContext context)
        {
            return new SortBuilder<T>(
                context.Request.Query.SortBy,
                context.Request.Query.SortDescending);
        }

        public static ISearchTerms SearchTerms<T>(this NancyContext context)
        {
            return new SearchBuilder<T>(context.Request.Query);
        }

        public static void SetupIdentityFromSession(this NancyContext context)
        {
            var sessionUser = context.Request.Session[IdentityKey] as IAuthorizedUser;

            if (sessionUser != null)
            {
                context.CurrentUser = new NancyIdentity(sessionUser);
            }
        }

        public static void SetupSession(this NancyContext context, IAuthorizedUser authorizedUser)
        {
            context.Request.Session[IdentityKey] = authorizedUser;
            context.CurrentUser = new NancyIdentity(authorizedUser);
        }

        public static IAuthorizedUser GetAuthorizedUser(this NancyContext context)
        {
            return context.Request.Session[IdentityKey] as IAuthorizedUser;
        }

        public static void ClearSession(this NancyContext context)
        {
            context.CurrentUser = null;
            context.Request.Session[IdentityKey] = null;
        }
    }
}
