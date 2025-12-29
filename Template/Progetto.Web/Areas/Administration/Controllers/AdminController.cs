using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progetto.Web.Features.Administration;
using System.Threading.Tasks;
using System;
using Template.Infrastructure;
using Template.Services;
using Template.Services.Shared;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Progetto.Web.Areas.Administration.Controllers
{

    [Area("Administration")]
    [Authorize(Roles = "Admin,TeamLeader")]
    public class AdminController : AuthenticatedBaseController
    {

        private readonly SharedService _sharedService;
        private readonly TemplateDbContext _context;

        public AdminController(SharedService sharedService, TemplateDbContext context)
        {
            _sharedService = sharedService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUserId = Guid.Empty;

            var model = new AdminFilterViewModel
            {
                Users = (await _sharedService.Query(new UsersIndexQuery
                {
                    IdCurrentUser = currentUserId,
                    Filter = null,
                    Paging = new Paging { PageSize = 1000 }
                })).Users,

                Projects = await _context.Projects.OrderBy(p => p.Code).ToListAsync(),
                Tasks = await _context.Tasks.OrderBy(t => t.Code).ToListAsync()
            };

            return View(model);
        }
    }
}
