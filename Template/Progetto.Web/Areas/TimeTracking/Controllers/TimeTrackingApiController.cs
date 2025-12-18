using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure;
using Template.Services;
using Template.Services.Shared.TimeTracking;

namespace Progetto.Web.Areas.TimeTracking.Controllers
{
    [Area("TimeTracking")]
    [ApiController]
    [Route("api/[area]/[controller]")]
    public class TimeTrackingApiController : ControllerBase
    {
        private readonly TemplateDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public TimeTrackingApiController(TemplateDbContext context, ICurrentUserService currenUser)
        {
            _context = context;
            _currentUser = currenUser;
        }

        [HttpGet("days-list")]
        public async Task<ActionResult<IEnumerable<object>>> GetDaysList([FromQuery] string mode)
        {
            var userId = _currentUser.UserId;
            DateTime from, to;
            var today = DateTime.Today;

            if (mode == "month")
            {
                from = new DateTime(today.Year, today.Month, 1);
                to = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            }
            else // "upto"
            {
                from = new DateTime(today.Year, 1, 1);
                to = today;
            }

            var entries = await _context.TimeEntries
                .Where(te => te.UserId == userId && te.Date >= from && te.Date <= to)
                .Include(te => te.Tags)
                .Include(te => te.Project)
                .ToListAsync();
            
            var days = new List<object>();
            for (var d = from; d <= to; d = d.AddDays(1))
            {
                var dayEntries = entries.Where(e => e.Date.Date == d.Date).ToList();
                decimal? totalHours = dayEntries.Any() ? dayEntries.Sum(e => e.HoursWorked) : (decimal?)null;
                string summary = dayEntries.Any() ? string.Join(", ", dayEntries.Select(e => e.Project.Name)) : "";

                days.Add(new
                {
                    date = d.ToString("yyyy-MM-dd"),
                    totalHours,
                    summary,
                    status = totalHours == null ? "not_reported" : "reported"
                });
            }

            return Ok(days);
        }

        [HttpGet("projects")]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var list = await _context.Projects.OrderBy(p => p.Code).ToListAsync();
            return Ok(list);
        }

        [HttpGet("tasks")]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var list = await _context.Tasks.OrderBy(p => p.Code).ToListAsync();
            return Ok(list);
        }
    }
}
