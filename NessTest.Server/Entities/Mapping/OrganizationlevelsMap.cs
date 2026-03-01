using Entities.Dbo;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Mapping
{
    public class OrganizationlevelsMap : ClassMap<Organizationlevels>
    {
        public OrganizationlevelsMap()
        {
            Table("OrganizationLevels");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("ID");
            Map(x => x.ParentId).Column("ParentID");
            Map(x => x.Name).Column("Name").Not.Nullable();
            Map(x => x.IsRowDeleted).Column("IsRowDeleted").Not.Nullable();
        }
    }
}
