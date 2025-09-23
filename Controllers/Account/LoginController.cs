using Microsoft.AspNetCore.Mvc;

namespace NoteFeature_App.Controllers.Account
{
    public class LoginController : Controller
    {
        [Route("/login")]
        [Route("/")]
        public IActionResult Index()
        {
            return View("~/Views/Account/Login/Index.cshtml");
        }
    }
}
