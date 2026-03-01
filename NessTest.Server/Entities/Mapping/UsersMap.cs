using Entities.Dbo;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Mapping
{
    public class UsersMap : ClassMap<Users>
    {
        public UsersMap()
        {
            Table("Users");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Assigned().Column("ID");
            References(x => x.Organizationlevels).Column("OrganizationLevel");
            References(x => x.Role).Column("Role");
            Map(x => x.Username).Column("UserName").Not.Nullable();
            Map(x => x.Email).Column("Email").Not.Nullable();
            Map(x => x.Phone).Column("Phone").Not.Nullable();
            Map(x => x.Managerid).Column("ManagerID");
            Map(x => x.Password).Column("Password").Not.Nullable();
            Map(x => x.Salt).Column("Salt").Not.Nullable();
            Map(x => x.IsTemporaryPassword).Column("IsTemporaryPassword").Not.Nullable();
            Map(x => x.IsActive).Column("IsActive").Not.Nullable();
            Map(x => x.CreateDate).Column("CreateDate").Not.Nullable();
            Map(x => x.LastUpdateDate).Column("LastUpdateDate").Not.Nullable();
        }
    }
}
