using System.Collections.Generic;

namespace Encore.Domain.Entities
{
    public class SystemUser : EntityBase, IAuthorizedUser
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public UserRole UserRole { get; set; }

        public List<ProjectToken> ProjectTokens { get; set; }
    }
}
