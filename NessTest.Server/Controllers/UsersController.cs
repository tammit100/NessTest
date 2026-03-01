using Entities.Dbo;
using Entities.Lookup;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(policyName: "CorsPolicy")]
    public class UsersController : ControllerBase
    {
        private readonly string _filePath = @"Data\users_new.json";

        // פונקציית עזר לקריאת הקובץ
        //private List<dynamic> GetUsersFromFile()
        //{
        //    if (!System.IO.File.Exists(_filePath)) return new List<dynamic>();
        //    var json = System.IO.File.ReadAllText(_filePath, System.Text.Encoding.UTF8);
        //    return JsonSerializer.Deserialize<List<dynamic>>(json) ?? new List<dynamic>();
        //}

        private List<Users> GetUsersFromFile()
        {
            if (!System.IO.File.Exists(_filePath)) return new List<Users>();

            var json = System.IO.File.ReadAllText(_filePath, System.Text.Encoding.UTF8);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<Users>>(json, options) ?? new List<Users>();
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
            // 1. קריאת המידע מהקובץ והמרתו לרשימת אובייקטים מסוג Users
            var jsonRaw = System.IO.File.ReadAllText(_filePath, System.Text.Encoding.UTF8);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<Users>>(jsonRaw, options);

            if (users == null) return NotFound();

            // 2. חיפוש המשתמש הספציפי
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null) return NotFound();

            // 3. עדכון השדות מתוך ה-JsonElement
            // שימוש ב-TryGetProperty כדי למנוע קריסה אם שדה חסר
            if (updatedUser.TryGetProperty("username", out var username))
                user.Username = username.GetString();

            if (updatedUser.TryGetProperty("email", out var email))
                user.Email = email.GetString();

            if (updatedUser.TryGetProperty("phone", out var phone))
                user.Phone = phone.GetString();

            if (updatedUser.TryGetProperty("isActive", out var isActive))
                user.IsActive = isActive.GetBoolean();

            // עדכון אובייקט ה-Role במידה ונשלח
            if (updatedUser.TryGetProperty("role", out var roleProp))
            {
                user.Role = new Role
                {
                    Code = roleProp.GetProperty("code").GetInt32(),
                    Description = roleProp.GetProperty("description").GetString()
                };
            }

            // עדכון תאריך עדכון אחרון
            user.LastUpdateDate = DateTime.Now;

            // 4. שמירה חזרה לקובץ עם תמיכה בעברית ועיצוב יפה
            var writeOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            var newJson = JsonSerializer.Serialize(users, writeOptions);
            System.IO.File.WriteAllText(_filePath, newJson, System.Text.Encoding.UTF8);

            return Ok(user);
        }


        [HttpPost]
        public IActionResult AddUser([FromBody] JsonElement newUser)
        {
            // 1. קריאת הנתונים הקיימים
            var jsonRaw = System.IO.File.ReadAllText(_filePath, System.Text.Encoding.UTF8);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = JsonSerializer.Deserialize<List<Users>>(jsonRaw, options) ?? new List<Users>();

            // 2. יצירת אובייקט משתמש חדש והשמת נתונים מה-JsonElement
            var userToAdd = new Users
            {
                Id = newUser.GetProperty("id").GetString(),
                Username = newUser.GetProperty("username").GetString(),
                Email = newUser.GetProperty("email").GetString(),
                Phone = newUser.GetProperty("phone").GetString(),
                IsActive = newUser.GetProperty("isActive").GetBoolean(),
                IsTemporaryPassword = newUser.TryGetProperty("isTemporaryPassword", out var isTemp) && isTemp.GetBoolean(),
                CreateDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            };

            // טיפול באובייקט ה-Role המקונן
            if (newUser.TryGetProperty("role", out var roleProp))
            {
                userToAdd.Role = new Role
                {
                    Code = roleProp.GetProperty("code").GetInt32(),
                    Description = roleProp.GetProperty("description").GetString()
                };
            }

            // 3. הוספה לרשימה
            users.Add(userToAdd);

            // 4. שמירה חזרה לקובץ עם תמיכה בעברית
            var writeOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            System.IO.File.WriteAllText(_filePath, JsonSerializer.Serialize(users, writeOptions), System.Text.Encoding.UTF8);

            return Ok(userToAdd);
        }


    }
}
