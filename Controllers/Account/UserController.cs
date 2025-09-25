using Microsoft.AspNetCore.Mvc;
using NoteFeature_App.Models.DTO;
using NoteFeature_App.Models.User;
using NoteFeature_App.Repositories;

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
            bool isValidUser = _userRepo.ValidateUser(email, password);

            if (!isValidUser)
            {
                ViewBag.Errors = new List<string> { "อีเมลหรือรหัสผ่านไม่ถูกต้อง" };

                return View();
            }

            return RedirectToAction("Index", "Note");
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

            _userRepo.AddUser(registerDTO);

            return RedirectToAction("login");
        }
        
    }
}
