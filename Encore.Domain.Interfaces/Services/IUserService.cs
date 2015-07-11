namespace Encore.Domain.Interfaces.Services
{
    using Encore.Domain.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    public interface IUserService
    {
        SystemUser AuthenticateUser(string name, string password);

        SystemUser GetUser(Guid userId);

        SystemUser UpdateUser(Guid userId, string name, string password, string email);

        SystemUser CreateUser(string name, string password, string email);
    }
}
