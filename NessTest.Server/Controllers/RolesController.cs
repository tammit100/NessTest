using AutoMapper;
using DataAccess;
using Entities.Lookup;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ISession = NHibernate.ISession;

namespace NessTest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(policyName: "CorsPolicy")]
    public class RolesController : Controller
    {

        private readonly ISession _session;
        private readonly IMapper _mapper;

        public RolesController(ISessionFactoryHelper sessionHelper, IMapper mapper)
        {
            _session = sessionHelper.GetSession();
            _mapper = mapper;
        }
        //[HttpGet]
        //public IActionResult GetRoles()
        //{
        //    string filePath = @"Data\roles.json";
        //    if (!System.IO.File.Exists(filePath)) return NotFound();

        //    var json = System.IO.File.ReadAllText(filePath, System.Text.Encoding.UTF8);
        //    var roles = JsonSerializer.Deserialize<List<dynamic>>(json);
        //    return Ok(roles);
        //}

        [HttpGet]
        public IActionResult GetRoles()
        {
            // שליפת כל הישויות מ-NHibernate
            var userEntities = _session.Query<Entities.Lookup.Role>().ToList();

            // המרה של כל הרשימה ל-DTOs בעזרת AutoMapper
            var roles = _mapper.Map<List<Models.RoleDto>>(userEntities);

            return Ok(roles);
        }

        [HttpGet("organizationlevels")]
        public IActionResult GetOrganizationlevelsMap()
        {
            // שליפת כל הישויות מ-NHibernate
            var userEntities = _session.Query<Entities.Dbo.Organizationlevels>().ToList();

            // המרה של כל הרשימה ל-DTOs בעזרת AutoMapper
            var organizationlevelsMap = _mapper.Map<List<Models.OrgLevelDto>>(userEntities);

            return Ok(organizationlevelsMap);
        }
    }

}
