using Microsoft.AspNetCore.Mvc;

namespace NoteFeature_App.Controllers.Home
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
