using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template.Services;
using Template.Services.Shared.TimeTracking.DTOs;

namespace Progetto.Web.Areas.Administration.Controllers
{
    [Area("Administration")]
    [ApiController]
    [Route("api/[area]/[controller]")]
    public class AdminApiController : ControllerBase
    {
        private readonly TemplateDbContext _context;

        public AdminApiController(TemplateDbContext context)
        {
            _context = context;
        }

        [HttpGet("getTimeEntries")]
        public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntriesAdmin([FromQuery] Guid userId, [FromQuery] Guid? projectId, [FromQuery] Guid? taskId, [FromQuery] string? queryText)
        {
            if (userId == Guid.Empty)
                return BadRequest("UserId is required.");

            var query = _context.TimeEntries
                .Include(t => t.Project)
                .Include(t => t.Task)
                .Where(t => t.UserId == userId)
                .AsQueryable();

            if (projectId.HasValue)
                query = query.Where(t => t.ProjectId == projectId.Value);

            if (taskId.HasValue)
                query = query.Where(t => t.TaskId == taskId.Value);

            if (!string.IsNullOrWhiteSpace(queryText))
            {
                var q = queryText.ToLower();

                query = query.Where(t =>
                    t.Notes.ToLower().Contains(q) ||
                    t.Project.Name.ToLower().Contains(q) ||
                    t.Task.Name.ToLower().Contains(q) ||
                    t.Date.ToString().Contains(q)
                );
            }

            var result = await query
                .OrderBy(t => t.Date)
                .Select(t => new TimeEntryDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    ProjectCode = t.Project.Code,
                    ProjectName = t.Project != null ? t.Project.Name : null,
                    TaskId = t.TaskId,
                    TaskCode = t.Task.Code,
                    TaskName = t.Task != null ? t.Task.Name : null,
                    Date = t.Date,
                    HoursWorked = t.HoursWorked, 
                    Notes = t.Notes,
                    Type = t.Type,
                    Tags = t.Tags.Select(tag => tag.Name).ToList()
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
