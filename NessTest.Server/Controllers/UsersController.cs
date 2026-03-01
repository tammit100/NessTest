using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System.Text.Json;

namespace TestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(policyName: "CorsPolicy")]
    public class UsersController : ControllerBase
    {
        private readonly string _filePath = @"Data\users.json";

        // פונקציית עזר לקריאת הקובץ
        private List<dynamic> GetUsersFromFile()
        {
            if (!System.IO.File.Exists(_filePath)) return new List<dynamic>();
            var json = System.IO.File.ReadAllText(_filePath, System.Text.Encoding.UTF8);
            return JsonSerializer.Deserialize<List<dynamic>>(json) ?? new List<dynamic>();
        }
        private void SaveUsersToFile(List<dynamic> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_filePath, json);
        }

        
        public UsersController()
        {
        }


        //[HttpGet]
        //public IActionResult GetUsers()
        //{
        //    // נתונים לדוגמה - בהמשך תמשוך אותם מה-DB
        //    var users = new List<object>
        //    {
        //        new { Id = "02345678", Name = "נורית ברוש", Position = "בודקת QA", CreatedDate = DateTime.Now.AddMonths(-5), IsActive = true },
        //        new { Id = "12345678", Name = "ישראל ישראלי", Position = "מפתח Fullstack", CreatedDate = DateTime.Now.AddDays(-10), IsActive = true },
        //        new { Id = "312456789", Name = "אבי כהן", Position = "מנהל מוצר", CreatedDate = DateTime.Now.AddYears(-1), IsActive = true },
        //        new { Id = "054321678", Name = "מיכל לוי", Position = "מעצבת UI/UX", CreatedDate = DateTime.Now.AddMonths(-2), IsActive = false },
        //        new { Id = "203456781", Name = "דניאל מזרחי", Position = "מפתח Backend", CreatedDate = DateTime.Now.AddDays(-30), IsActive = true },
        //        new { Id = "039876542", Name = "רחל אברהם", Position = "ראש צוות", CreatedDate = DateTime.Now.AddYears(-2), IsActive = true },
        //        new { Id = "445566778", Name = "יוסי לוין", Position = "מפתח Frontend", CreatedDate = DateTime.Now.AddDays(-2), IsActive = true }
        //    };

        //    return Ok(users);
        //}

        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(GetUsersFromFile());
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, [FromBody] JsonElement updatedUser)
        {
            var jsonRaw = System.IO.File.ReadAllText(_filePath, System.Text.Encoding.UTF8);
            var users = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonRaw);
            if (users != null)
            {
                // חיפוש המשתמש לפי ה-id (באותיות קטנות)
                var user = users.FirstOrDefault(u => u["id"].ToString() == id);

                if (user == null) return NotFound();

                // עדכון הערכים במילון
                user["name"] = updatedUser.GetProperty("name").GetString();
                user["position"] = updatedUser.GetProperty("position").GetInt32();
                user["phone"] = updatedUser.GetProperty("phone").GetString();
                user["email"] = updatedUser.GetProperty("email").GetString();
                user["isActive"] = updatedUser.GetProperty("isActive").GetBoolean();

                // הגדרות לשמירה: עברית קריאה ועיצוב יפה (Indented)
                var options = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };

                var newJson = JsonSerializer.Serialize(users, options);
                System.IO.File.WriteAllText(_filePath, newJson, System.Text.Encoding.UTF8);
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] JsonElement newUser)
        {
            var jsonRaw = System.IO.File.ReadAllText(_filePath, System.Text.Encoding.UTF8);
            var users = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonRaw);

            // יצירת מילון חדש עבור המשתמש
            var newUserDict = new Dictionary<string, object>
            {
                ["id"] = newUser.GetProperty("id").GetString(),
                ["name"] = newUser.GetProperty("name").GetString(),
                ["position"] = newUser.GetProperty("position").GetInt32(),
                ["phone"] = newUser.GetProperty("phone").GetString(),
                ["email"] = newUser.GetProperty("email").GetString(),
                ["isActive"] = newUser.GetProperty("isActive").GetBoolean(),
                ["createdDate"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            users.Add(newUserDict);

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            System.IO.File.WriteAllText(_filePath, JsonSerializer.Serialize(users, options), System.Text.Encoding.UTF8);

            return Ok(newUserDict);
        }

    }
}
