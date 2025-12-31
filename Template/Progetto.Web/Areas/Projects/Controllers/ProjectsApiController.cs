using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Template.Services;
using Template.Services.Shared.WorkManagement.DTOs;

namespace Progetto.Web.Areas.Projects.Controllers
{
    [Area("Projects")]
    [ApiController]
    [Route("api/[area]/[controller]")]
    public class ProjectsApiController : ControllerBase
    {
        private readonly TemplateDbContext _context;

        public ProjectsApiController(TemplateDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectsWithTasks()
        {
            var projects = await _context.Projects
                .Include(p => p.Tasks)
                .OrderBy(p => p.Code)
                .Select(p => new ProjectWithTasksDto
                {
                    ProjectId = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    Tasks = _context.Tasks
                        .Where(t => t.ProjectId == p.Id)
                        .OrderBy(t => t.Code)
                        .Select(t => new TaskDto
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Code = t.Code
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(projects);
        }    
    }
}
