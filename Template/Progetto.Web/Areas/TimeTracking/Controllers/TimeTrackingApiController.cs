using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure;
using Template.Services;
using Template.Services.Shared.TimeTracking;
using Template.Services.Shared.TimeTracking.DTOs;

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

        [HttpGet("calendar-status")]
        public async Task<IActionResult> GetCalendarStatus(int year, int month)
        {
            var userId = _currentUser.UserId;

            var firstDay = new DateTime(year, month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            // Estraggo tutte le time entries dell'utente per quel mese
            var entries = await _context.TimeEntries
                .Where(te => te.UserId == userId && te.Date >= firstDay && te.Date <= lastDay)
                .ToListAsync();

            // Build della lista dei giorni con state
            var result = Enumerable.Range(1, lastDay.Day).Select(day =>
            {
                var date = new DateTime(year, month, day);
                var totalHours = entries
                    .Where(te => te.Date.Date == date.Date)
                    .Sum(te => te.HoursWorked);

                string state;
                if (IsWeekend(date) || IsHoliday(date))
                    state = "non_working";           // Weekend o festività
                else if (totalHours == 0)
                    state = "missing";               // Nessuna ora registrata Default
                else if (totalHours > 0 && totalHours < 8)
                    state = "insufficient";               // Ore parziali (<8)
                else if (totalHours == 8)
                    state = "reported";              // Ore complete
                else
                    state = "overtime";          // straordinari

                return new
                {
                    date = date.ToString("yyyy-MM-dd"),
                    state
                };
            }).ToList();

            return Ok(result);
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
                .Include(te => te.Task)
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
                    status = totalHours == null ? "not_reported" : "reported",

                    entries = dayEntries.Select(e => new
                    {
                        id = e.Id,
                        hours = e.HoursWorked,
                        description = e.Notes,
                        projectId = e.ProjectId,
                        project = e.Project?.Name,
                        projectCode = e.Project?.Code,
                        taskId = e.TaskId,
                        task = e.Task?.Name,
                        taskCode = e.Task?.Code,
                        type = e.Type
                    })
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

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private bool IsHoliday(DateTime date)
        {
            var holidays = new[]
            {
                new DateTime(date.Year, 1, 1),
                new DateTime(date.Year, 1, 6),
                new DateTime(date.Year, 4, 25),
                new DateTime(date.Year, 5, 1),
                new DateTime(date.Year, 6, 2),
                new DateTime(date.Year, 8, 15),
                new DateTime(date.Year, 11, 1),
                new DateTime(date.Year, 12, 8),
                new DateTime(date.Year, 12, 25),
                new DateTime(date.Year, 12, 26)
            };
            return holidays.Contains(date.Date);
        }
    }
}
