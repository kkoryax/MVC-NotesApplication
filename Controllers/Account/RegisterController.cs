using Microsoft.AspNetCore.Mvc;
using NoteFeature_App.Controllers.Note;
using NoteFeature_App.Models.User;

namespace NoteFeature_App.Controllers.Account
{
    [Route("[controller]/[action]")]
    public class RegisterController : Controller
    {
        [Route("/register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Account/Register/Register.cshtml");
        }
        [HttpPost]
        public IActionResult Register(UserModel user)
        {
            return RedirectToAction(nameof(NoteController.Index), "Note");
        }
    }
}
