using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template.Infrastructure;
using Template.Services;
using Template.Services.Shared;
using Template.Services.Shared.TimeTracking;
using Template.Services.Shared.WorkManagement.DTOs;

namespace Progetto.Web.Features.WorkManagement
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkManagementController : ControllerBase
    {
        private readonly TemplateDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public WorkManagementController(TemplateDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        [HttpPost("addProject")]
        public IActionResult CreateProject([FromBody] CreateProjectRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_currentUser.IsAuthenticated)
                return Unauthorized();

            //Check codice progetto univoco
            if (_context.Projects.Any(p => p.Code == req.Code))
                return Conflict("Il codice progetto è già in uso.");

            var userId = _currentUser.UserId;

            var project = new Project
            {
                Id = Guid.NewGuid(),
                Name = req.Name,
                Code = req.Code, 
                CreatedByUserId = userId
            };

            _context.Projects.Add(project);
            _context.SaveChanges();

            return Ok(project);
        }

        [HttpPut("editProject/{id:guid}")]
        public async Task<IActionResult> EditProject(Guid id, [FromBody] EditProjectRequest req)
        {
            if (id != req.Id)
                return BadRequest("L'ID nel percorso non corrisponde all'ID nel body.");

            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            var codeExists = await _context.Projects
                .AnyAsync(p => p.Code == req.Code && p.Id != id);

            if (codeExists)
                return Conflict("Il codice progetto è già in uso.");

            project.Name = req.Name;
            project.Code = req.Code;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Progetto modificato con successo.",
                id = project.Id
            });
        }

        [HttpDelete("deleteProject/{id:guid}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
                return NotFound();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Progetto eliminato.",
                id
            });
        }

        [HttpPost("projects/{projectId:guid}/addTask")]
        public async Task<IActionResult> CreateTask(Guid projectId,[FromBody] CreateTaskRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_currentUser.IsAuthenticated)
                return Unauthorized();

            //codice univoco
            if (_context.Tasks.Any(t => t.Code == req.Code))
                return Conflict("Il codice task è già in uso.");

            var creatorId = _currentUser.UserId;

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Name = req.Name,
                Code = req.Code,
                CreatedByUserId = creatorId,
                ProjectId = projectId
            };

            _context.Tasks.Add(task);

            if (req.AssignedUserIds?.Any() == true)
            {
                foreach (var userId in req.AssignedUserIds)
                {
                    var exists = await _context.Users.AnyAsync(u => u.Id == userId);
                    if (!exists)
                        return BadRequest($"Utente {userId} inesistente.");

                    _context.TaskAssignments.Add(new TaskAssignment
                    {
                        TaskId = task.Id,
                        UserId = userId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = task.Id,
                name = task.Name,
                code = task.Code,
                createdByUserId = task.CreatedByUserId,
                projectId = task.ProjectId,
                assignedUsers = req.AssignedUserIds
            });
        }

        [HttpPut("projects/{projectId:guid}/editTask/{id:guid}")]
        public async Task<IActionResult> EditTask(Guid id, [FromBody] EditTaskRequest req)
        {
            if (id != req.Id)
                return BadRequest("L'id nel percorso non corrisponde all'id nel body.");

            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound("Task non trovato.");

            var codeExists = await _context.Tasks
                .AnyAsync(t => t.Code == req.Code && t.Id != id);

            if (codeExists)
                return Conflict("Il codice task è già in uso da un altro task.");

            task.Name = req.Name;
            task.Code = req.Code;
            task.ProjectId = req.ProjectId;

            //recupero delle assegnazioni esistenti
            var existingAssignments = await _context.TaskAssignments
                .Where(a => a.TaskId == id)
                .Select(a => a.UserId)
                .ToListAsync();

            var newAssignments = req.AssignedUserIds ?? new List<Guid>();

            var toAdd = newAssignments.Except(existingAssignments).ToList();

            var toRemove = existingAssignments.Except(newAssignments).ToList();

            //aggiunta delle nuove asssegnazioni
            foreach (var userId in toAdd)
            {
                var exists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!exists)
                    return BadRequest($"Utente {userId} inesistente.");

                _context.TaskAssignments.Add(new TaskAssignment
                {
                    TaskId = id,
                    UserId = userId
                });
            }

            //Rimozione utenti non più assegnati
            if (toRemove.Any())
            {
                var assignmentsToRemove = _context.TaskAssignments
                    .Where(a => a.TaskId == id && toRemove.Contains(a.UserId));

                _context.TaskAssignments.RemoveRange(assignmentsToRemove);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task modificato con successo.",
                id = task.Id
            }); 
        }

        [HttpDelete("projects/{projectId:guid}/deleteTask/{id:guid}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound("Task non trovato.");

            //rimozione delle assegnazioni
            var assignments = _context.TaskAssignments.Where(a => a.TaskId == id);
            _context.TaskAssignments.RemoveRange(assignments);

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Task eliminato.",
                id
            });
        }
    } 
}
