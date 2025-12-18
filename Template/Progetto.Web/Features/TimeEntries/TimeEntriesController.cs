using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Template.Services.Shared.TimeTracking;
using System.Threading.Tasks;
using Template.Services;
using System.Linq;
using System;
using System.Collections;
using Template.Services.Shared.TimeTracking.DTOs;
using System.Collections.Generic;
using Template.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Progetto.Web.Features.TimeEntries
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimeEntriesController : ControllerBase
    {
        private readonly TemplateDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public TimeEntriesController(TemplateDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.TimeEntries.ToListAsync();

            return Ok(items);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeEntryDto>>> GetTimeEntries([FromQuery] DateTime? date, [FromQuery] string month, [FromQuery] Guid? projectId, [FromQuery] Guid? taskId)
        {
            var userId = _currentUser.UserId;

            var query = _context.TimeEntries
                .Include(t => t.Project)
                .Include(t => t.Task)
                .Where(t => t.UserId == userId)
                .AsQueryable();

            if (date.HasValue)
                query = query.Where(t => t.Date.Date == date.Value.Date);

            if (!string.IsNullOrEmpty(month))
            {
                var parsed = DateTime.Parse(month + "-01");
                query = query.Where(t => t.Date.Month == parsed.Month && t.Date.Year == parsed.Year);
            }

            if (projectId.HasValue)
                query = query.Where(t => t.ProjectId == projectId.Value);

            if (taskId.HasValue)
                query = query.Where(t => t.TaskId == taskId.Value);

            var result = await query
                .OrderBy(t => t.Date)
                .Select(t => new TimeEntryDto
                {
                    Id = t.Id,
                    ProjectId = t.ProjectId,
                    ProjectName = t.Project != null ? t.Project.Name : null,
                    TaskId = t.TaskId,
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

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(TimeEntryCreateDto dto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _currentUser.UserId;

            if (userId == Guid.Empty)
                return Unauthorized();

            var entry = new TimeEntry
            {
                UserId = userId,
                ProjectId = dto.ProjectId,
                TaskId = dto.TaskId,
                Date = dto.Date.Date,
                HoursWorked = dto.HoursWorked,
                Notes = dto.Notes,
                Type = dto.Type
            };

            _context.TimeEntries.Add(entry);
            await _context.SaveChangesAsync();

            return Ok(new { id = entry.Id });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, TimeEntryUpdateDto dto)
        {
            var userId = _currentUser.UserId;

            var entry = await _context.TimeEntries
                .Where(t => t.UserId == userId && t.Id == id)
                .FirstOrDefaultAsync();

            if (entry == null) return NotFound();


            entry.ProjectId = dto.ProjectId;
            entry.TaskId = dto.TaskId;
            entry.Date = dto.Date.Date;
            entry.HoursWorked = dto.HoursWorked;
            entry.Notes = dto.Notes;
            entry.Type = dto.Type;

            await _context.SaveChangesAsync();

            return Ok(new { id = entry.Id });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var userId = _currentUser.UserId;

            var entry = await _context.TimeEntries
                .Where(t => t.UserId == userId && t.Id == id)
                .FirstOrDefaultAsync();

            if (entry == null) return NotFound();

            _context.TimeEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("summary")]
        public async Task<ActionResult<List<TimeSummaryDto>>> GetSummary([FromBody] TimeSummaryFilterDto filter)
        {
            var userId = _currentUser.UserId;
            var timeEntries = await _context.TimeEntries
                .Where(t => t.UserId == userId &&
                            t.Date >= filter.From &&
                            t.Date <= filter.To &&
                            (filter.ProjectId == null || t.ProjectId == filter.ProjectId))
                .Include(t => t.ProjectId)
                .Include(t => t.Tags)
                .ToListAsync();

            var summary =
                timeEntries
                    .GroupBy(t => t.Project)
                    .Select(projectGroup => new TimeSummaryDto
                    {
                        ProjectId = projectGroup.Key.Id,
                        ProjectName = projectGroup.Key.Name,
                        TotalHours = projectGroup.Sum(t => t.HoursWorked),
                        Tags = projectGroup
                            .SelectMany(t => t.Tags)
                            .GroupBy(tag => tag.Id)
                            .Select(tagGroup => new TagSummaryDto
                            {
                                TagId = tagGroup.Key,
                                TagName = tagGroup.First().Name,
                                Hours = projectGroup
                                    .Where(t => t.Tags.Any(tag => tag.Id == tagGroup.Key))
                                    .Sum(t => t.HoursWorked)
                            })
                            .ToList()
                    })
                    .ToList();

            return Ok(summary);
        }
    }
}
