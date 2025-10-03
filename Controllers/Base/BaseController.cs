using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NoteFeature_App.Repositories;

namespace NoteFeature.Controllers.Base
{
    public class BaseController : Controller
    {
        protected string? BaseUrl = null;

        public BaseController()
        {
        }

        protected string RenderRazorViewtoString(Controller controller, string viewName, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine? viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewEngineResult = viewEngine.FindView(controller.ControllerContext, viewName, false);
                ViewContext viewContext = new ViewContext
                (
                    controller.ControllerContext,
                    viewEngineResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewEngineResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}

