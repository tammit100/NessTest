using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.mapping
{
    public class EntityToModel  : Profile
    {
        public EntityToModel()
        {
            CreateMap<Entities.Dbo.Users, Models.Users>().ReverseMap();

            CreateMap<Entities.Lookup.Role, Models.RoleDto>().ReverseMap();

            CreateMap<Entities.Dbo.Organizationlevels, Models.OrgLevelDto>().ReverseMap();
        }
    }
}
