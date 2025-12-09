using Microsoft.AspNetCore.Mvc;

namespace Progetto.Web.Areas.TimeTracking.Controllers
{
    [Area("TimeTracking")]
    public class TimeTrackingController : AuthenticatedBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
