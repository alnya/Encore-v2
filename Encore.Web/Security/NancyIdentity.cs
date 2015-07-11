namespace Encore.Web.Security
{
    using Encore.Domain.Entities;
    using Nancy.Security;
    using System.Collections.Generic;

    public class NancyIdentity : IUserIdentity
    {
        private readonly IAuthorizedUser authorizedUser;

        public NancyIdentity(IAuthorizedUser authorizedUser)
        {
            this.authorizedUser = authorizedUser;
        }

        public IEnumerable<string> Claims
        {
            get { return new List<string>(); }
        }

        public string UserName
        {
            get { return authorizedUser.Name; }
        }
    }
}
