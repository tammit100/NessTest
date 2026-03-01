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
            CreateMap<Entities.Dbo.Users, Models.Users>()
            // אומרים לאוטומאפר: קח את הערך מ-src.Role.Code ושים אותו ב-dest.RoleId
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role.Code))
                // אותו דבר עבור Organizationlevels אם צריך
                .ForMember(dest => dest.OrganizationlevelsId, opt => opt.MapFrom(src => src.Organizationlevels.Id))
                .ReverseMap();

            CreateMap<Entities.Lookup.Role, Models.RoleDto>().ReverseMap();

            CreateMap<Entities.Dbo.Organizationlevels, Models.OrgLevelDto>().ReverseMap();
        }
    }
}
