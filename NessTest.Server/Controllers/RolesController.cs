using Entities.Lookup;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace NessTest.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(policyName: "CorsPolicy")]
    public class RolesController : Controller
    {
        [HttpGet]
        public IActionResult GetRoles()
        {
            string filePath = @"Data\roles.json";
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var json = System.IO.File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            var roles = JsonSerializer.Deserialize<List<dynamic>>(json);
            return Ok(roles);
        }
    }

}
