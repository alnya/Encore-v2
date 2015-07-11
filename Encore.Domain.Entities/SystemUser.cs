namespace Encore.Domain.Entities
{
    using System;

    public class SystemUser : EntityBase, IAuthorizedUser
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }
    }
}
