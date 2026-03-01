using Entities.Lookup;
using FluentNHibernate.Mapping;

namespace Entities.Mapping
{
    public class RolesMap : ClassMap<Role>
    {
        public RolesMap()
        {
            Schema("LOOKUP");
            Table("Roles");
            LazyLoad();
            Id(x => x.Code).GeneratedBy.Identity().Column("Code");
            Map(x => x.Description).Column("Description").Not.Nullable();
        }
    }
}

