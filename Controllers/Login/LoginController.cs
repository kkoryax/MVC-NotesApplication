using Microsoft.AspNetCore.Mvc;

namespace NoteFeature_App.Controllers.Login
{
    public class LoginController : Controller
    {
        [Route("/login")]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
