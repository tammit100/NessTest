using AutoMapper;
using DataAccess;
using Entities.Dbo;
using Entities.Lookup;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ISession = NHibernate.ISession;

namespace TestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors(policyName: "CorsPolicy")]
    public class UsersController : ControllerBase
    {
        private readonly string _filePath = @"Data\users_new.json";
        private readonly ISession _session;
        private readonly IMapper _mapper;

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
        
        public UsersController(ISessionFactoryHelper sessionHelper, IMapper mapper)
        {
            _session = sessionHelper.GetSession();
            _mapper = mapper;
        }

        //[HttpGet]
        //public IActionResult GetUsers()
        //{
        //    return Ok(GetUsersFromFile());
        //}

        [HttpGet]
        public IActionResult GetUsers()
        {
            // שליפת כל הישויות מ-NHibernate
            var userEntities = _session.Query<Entities.Dbo.Users>().ToList();
            userEntities = userEntities.Where(w => IsUserWellFormed(w)).ToList();

            // המרה של כל הרשימה ל-DTOs בעזרת AutoMapper
            var users = _mapper.Map<List<Models.Users>>(userEntities);
           
            return Ok(users);
        }

        private bool IsUserWellFormed(Entities.Dbo.Users user)
        {
            if (user.Organizationlevels == null || user.Organizationlevels.Id <= 0)
                return false;

            if (user.Role == null || user.Role.Code == 0)
                return false;

            // 2. בדיקת Email בסיסית
            if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
                return false;

            // 3. בדיקת טלפון (לפחות 9 תווים)
            if (string.IsNullOrWhiteSpace(user.Phone) || user.Phone.Length < 9)
                return false;

            return true;
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(string id)
        {
            var userEntity = _session.Get<Entities.Dbo.Users>(id);
            if (userEntity == null || !IsUserWellFormed(userEntity)) return NotFound();

            // המרה מישות למודל לפני החזרה ל-Client
            var user = _mapper.Map<Models.Users>(userEntity);
            return Ok(user);
        }

        //[HttpPut("{id}")]
        //public IActionResult UpdateUser(string id, [FromBody] JsonElement updatedUser)
        //{
        //    // 1. קריאת המידע מהקובץ והמרתו לרשימת אובייקטים מסוג Users
        //    var jsonRaw = System.IO.File.ReadAllText(_filePath, System.Text.Encoding.UTF8);
        //    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //    var users = JsonSerializer.Deserialize<List<Users>>(jsonRaw, options);

        //    if (users == null) return NotFound();

        //    // 2. חיפוש המשתמש הספציפי
        //    var user = users.FirstOrDefault(u => u.Id == id);
        //    if (user == null) return NotFound();

        //    // 3. עדכון השדות מתוך ה-JsonElement
        //    // שימוש ב-TryGetProperty כדי למנוע קריסה אם שדה חסר
        //    if (updatedUser.TryGetProperty("username", out var username))
        //        user.Username = username.GetString();

        //    if (updatedUser.TryGetProperty("email", out var email))
        //        user.Email = email.GetString();

        //    if (updatedUser.TryGetProperty("phone", out var phone))
        //        user.Phone = phone.GetString();

        //    if (updatedUser.TryGetProperty("isActive", out var isActive))
        //        user.IsActive = isActive.GetBoolean();

        //    // עדכון אובייקט ה-Role במידה ונשלח
        //    if (updatedUser.TryGetProperty("role", out var roleProp))
        //    {
        //        user.Role = new Role
        //        {
        //            Code = roleProp.GetProperty("code").GetInt32(),
        //            Description = roleProp.GetProperty("description").GetString()
        //        };
        //    }

        //    // עדכון תאריך עדכון אחרון
        //    user.LastUpdateDate = DateTime.Now;

        //    // 4. שמירה חזרה לקובץ עם תמיכה בעברית ועיצוב יפה
        //    var writeOptions = new JsonSerializerOptions
        //    {
        //        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        //        WriteIndented = true
        //    };

        //    var newJson = JsonSerializer.Serialize(users, writeOptions);
        //    System.IO.File.WriteAllText(_filePath, newJson, System.Text.Encoding.UTF8);

        //    return Ok(user);
        //}


        [HttpPut("{id}")]
        public IActionResult UpdateUser(string id, [FromBody] Models.Users userDto)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var userEntity = _session.Get<Entities.Dbo.Users>(id);
                    if (userEntity == null) return NotFound();

                    // מעדכן את הישות הקיימת בנתונים החדשים מה-DTO
                    _mapper.Map(userDto, userEntity);
                    userEntity.LastUpdateDate = DateTime.Now;

                    _session.Update(userEntity);
                    transaction.Commit();

                    return Ok(_mapper.Map<Models.Users>(userEntity));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }


        //[HttpPost]
        //public IActionResult AddUser([FromBody] JsonElement newUser)
        //{
        //    // 1. קריאת הנתונים הקיימים
        //    var jsonRaw = System.IO.File.ReadAllText(_filePath, System.Text.Encoding.UTF8);
        //    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //    var users = JsonSerializer.Deserialize<List<Users>>(jsonRaw, options) ?? new List<Users>();

        //    // 2. יצירת אובייקט משתמש חדש והשמת נתונים מה-JsonElement
        //    var userToAdd = new Users
        //    {
        //        Id = newUser.GetProperty("id").GetString(),
        //        Username = newUser.GetProperty("username").GetString(),
        //        Email = newUser.GetProperty("email").GetString(),
        //        Phone = newUser.GetProperty("phone").GetString(),
        //        IsActive = newUser.GetProperty("isActive").GetBoolean(),
        //        IsTemporaryPassword = newUser.TryGetProperty("isTemporaryPassword", out var isTemp) && isTemp.GetBoolean(),
        //        CreateDate = DateTime.Now,
        //        LastUpdateDate = DateTime.Now
        //    };

        //    // טיפול באובייקט ה-Role המקונן
        //    if (newUser.TryGetProperty("role", out var roleProp))
        //    {
        //        userToAdd.Role = new Role
        //        {
        //            Code = roleProp.GetProperty("code").GetInt32(),
        //            Description = roleProp.GetProperty("description").GetString()
        //        };
        //    }

        //    // 3. הוספה לרשימה
        //    users.Add(userToAdd);

        //    // 4. שמירה חזרה לקובץ עם תמיכה בעברית
        //    var writeOptions = new JsonSerializerOptions
        //    {
        //        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        //        WriteIndented = true
        //    };

        //    System.IO.File.WriteAllText(_filePath, JsonSerializer.Serialize(users, writeOptions), System.Text.Encoding.UTF8);

        //    return Ok(userToAdd);
        //}

        [HttpPost]
        public IActionResult AddUser([FromBody] Models.Users user)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var userEntity = _mapper.Map<Entities.Dbo.Users>(user);
                    userEntity.CreateDate = DateTime.Now;
                    userEntity.LastUpdateDate = DateTime.Now;

                    _session.Save(userEntity);
                    transaction.Commit();

                    return Ok(_mapper.Map<Models.Users>(userEntity));
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(ex.Message);
                }
            }
        }


    }
}
