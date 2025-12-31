using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Template.Services;

namespace Progetto.Web.Areas.Projects.Controllers
{
    [Area("Projects")]
    [Authorize(Roles = "Admin,TeamLeader")]
    public class ProjectsController : AuthenticatedBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
