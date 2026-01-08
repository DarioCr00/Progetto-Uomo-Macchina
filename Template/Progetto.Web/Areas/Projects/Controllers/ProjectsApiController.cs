using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progetto.Web.Infrastructure;
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
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Code,
                    p.CreatedByUserId,
                    ProjectCreator = _context.Users
                        .Where(u => u.Id == p.CreatedByUserId)
                        .Select(u => new { u.FirstName, u.LastName, u.Email })
                        .FirstOrDefault(),
                    Tasks = p.Tasks
                        .OrderBy(t => t.Code)
                        .Select(t => new
                        {
                            t.Id,
                            t.Name,
                            t.Code,
                            t.ProjectId,
                            t.CreatedByUserId,
                            TaskCreator = _context.Users
                                .Where(u => u.Id == t.CreatedByUserId)
                                .Select(u => new { u.FirstName, u.LastName, u.Email })
                                .FirstOrDefault(),
                            AssignedUsers = _context.TaskAssignments
                                .Where(ta => ta.TaskId == t.Id)
                                .Select(ta => new
                                {
                                    ta.User.Id,
                                    ta.User.FirstName,
                                    ta.User.LastName,
                                    ta.User.Email
                                }).ToList()
                        }).ToList()
                })
                .ToListAsync();

            var result = projects.Select(p => new ProjectWithTasksDto
            {
                ProjectId = p.Id,
                Name = p.Name,
                Code = p.Code,
                CreatedByUserId = p.CreatedByUserId,
                CreatedByUserName = p.ProjectCreator != null ? $"{p.ProjectCreator.FirstName} {p.ProjectCreator.LastName}" : "N/A",
                CreatedByUserGravatar = p.ProjectCreator?.Email.ToGravatarUrl(ToGravatarUrlExtension.DefaultGravatar.Identicon, null),
                Tasks = p.Tasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Code = t.Code,
                    ProjectId = t.ProjectId,
                    CreatedByUserId = t.CreatedByUserId,
                    CreatedByUserName = t.TaskCreator != null ? $"{t.TaskCreator.FirstName} {t.TaskCreator.LastName}" : "N/A",
                    CreatedByUserGravatar = t.TaskCreator?.Email.ToGravatarUrl(ToGravatarUrlExtension.DefaultGravatar.Identicon, null),
                    AssignedUsers = t.AssignedUsers.Select(au => new UserDto
                    {
                        Id = au.Id,
                        FullName = $"{au.FirstName} {au.LastName}",
                        Email = au.Email
                    }).ToList()
                }).ToList()
            }).ToList();

            return Ok(result);
        }
    }
}
