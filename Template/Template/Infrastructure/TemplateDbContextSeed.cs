using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template.Services;
using Template.Services.Shared.TimeTracking;

namespace Template.Infrastructure
{
    public static class TemplateDbContextSeed
    {
        public static async Task Seed(TemplateDbContext context)
        {
            context.Database.EnsureCreated();

            //mantiene il seeding degli utenti già esistenti se presente
            DataGenerator.InitializeUsers(context);


            //crea degli esempi se non esistono dei veri
            if(!context.Projects.Any())
            {
                context.Projects.AddRange(
                    new Project { Id = Guid.NewGuid(), Code = "C0001", Name = "Commessa Cliente A"},
                    new Project { Id = Guid.NewGuid(), Code = "C0002", Name = "Commessa Cliente B"}
                );
                context.SaveChanges();
            }

            if (!context.Tags.Any())
            {
                context.Tags.AddRange(
                    new Tag { Name = "Meeting" },
                    new Tag { Name = "Sviluppo" },
                    new Tag { Name = "Bugfix" },
                    new Tag { Name = "Analisi" }
                );

                await context.SaveChangesAsync();
            }

            if(!context.Tasks.Any())
            {
                context.Tasks.AddRange(
                    new TaskItem { Id= Guid.NewGuid(), Code = "TSK001", Name = "Analisi tecnica"},
                    new TaskItem { Id = Guid.NewGuid(), Code = "TSK002", Name = "Sviluppo API" },
                    new TaskItem { Id = Guid.NewGuid(), Code = "TSK003", Name = "Testing" }
                );

                await context.SaveChangesAsync();
            }

            //Demo per TimeEntries
            if (!context.TimeEntries.Any())
            {
                var user = context.Users.FirstOrDefault();
                var project = context.Projects.FirstOrDefault();
                var task = context.Tasks.FirstOrDefault();

                var sviluppoTag = context.Tags.First(t => t.Name == "Sviluppo");

                if (user != null && project != null)
                {
                    context.TimeEntries.Add(new TimeEntry
                    {
                        UserId = user.Id,
                        ProjectId = project.Id,
                        TaskId = task.Id,
                        Date = new DateTime(2025, 12, 15),
                        HoursWorked = 8m,
                        Notes = "Lavoro di prova",
                        Type = WorkType.Normal,
                        Tags = new List<Tag> { sviluppoTag }
                    });
                    context.SaveChanges();
                }
            }           
        }
    }
}
