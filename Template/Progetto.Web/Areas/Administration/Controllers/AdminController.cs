using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Progetto.Web.Areas.Administration.Controllers
{

    [Area("Administration")]
    [Authorize(Roles = "Admin,TeamLeader")]
    public class AdminController : AuthenticatedBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
