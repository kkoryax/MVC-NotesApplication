using Microsoft.AspNetCore.Mvc;
using NoteFeature_App.Models.DTO;
using NoteFeature_App.Models.User;
using NoteFeature_App.Repositories;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace NoteFeature_App.Controllers.Account
{
    public class UserController : Controller
    {
        private readonly IUserRepo _userRepo;
        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        [Route("login")]
        [Route("/")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [Route("login")]
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.Errors = errors;

                return View();
            }
            
            // ตรวจสอบ user และดึงข้อมูล user
            var user = _userRepo.GetUserByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ViewBag.Errors = new List<string> { "อีเมลหรือรหัสผ่านไม่ถูกต้อง" };
                return View();
            }

            var token = CreateToken(user);
            
            Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddMinutes(60)
            });

            return RedirectToAction("Index", "Note");
        }

        private string CreateToken(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "User"),
                new Claim("UserId", user.UserId.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong!")
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "NoteFeatureApp",
                audience: "NoteFeatureAppUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Route("register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [Route("register")]
        [HttpPost]
        public IActionResult Register(RegisterDto? registerDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                ViewBag.Errors = errors;

                return View();
            }

            bool result = _userRepo.AddUser(registerDTO);

            if (!result)
            {
                ViewBag.Errors = new List<string> { "อีเมลนี้ถูกใช้งานแล้วในระบบ" };
                return View();
            }

            return RedirectToAction("UserManager");
        }

        [Route("logout")]
        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            return RedirectToAction("Login");
        }
        [Route("user-manager")]
        [HttpGet]
        public IActionResult UserManager()
        {

            var users = _userRepo.GetAllUser();

             return View(users);
        }

        [Route("/get-user-list")]
        [HttpGet]
        public JsonResult GetNoteList(UserPagination pagination)
        {
            try
            {
                if (pagination == null) pagination = new UserPagination();
                pagination.Offset = pagination.Page <= 1 ? 0 : pagination.Offset;

                var result = _userRepo.GetListUserPagination(pagination);

                var usersDto = result.Users.Select(n => new
                {
                    userId = n.UserId,
                    email = n.Email,
                    role = n.Role,
                    createdAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                }).ToList();

                return Json(new
                {
                    success = true,
                    users = usersDto,
                    total = result.Total
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = InnerException(ex)
                });
            }
        }
        protected string InnerException(Exception ex)
        {
            return (ex.InnerException != null) ? InnerException(ex.InnerException) : ex.Message;
        }

        [Route("user-manager/delete/{userId}")]
        public IActionResult Delete(Guid? userId)
        {
            if (userId == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                var users = _userRepo.GetAllUser();
                ViewBag.Users = users;
                ViewBag.Errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
                return View("UserManager");
            }
            _userRepo.DeleteUser(userId);
            return RedirectToAction("UserManager");
        }

    }
}
