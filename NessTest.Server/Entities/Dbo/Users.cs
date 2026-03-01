using Entities.Lookup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Dbo
{
    public class Users
    {
        public virtual string Id { get; set; }
        public virtual Organizationlevels Organizationlevels { get; set; }
        public virtual string Username { get; set; }
        public virtual Role Role { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Managerid { get; set; }
        public virtual string Password { get; set; }
        public virtual string Salt { get; set; }
        public virtual bool IsTemporaryPassword { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime LastUpdateDate { get; set; }
    }
}
