using System;
using System.Linq;
using Template.Services;
using Template.Services.Shared;
using Template.Services.Shared.TimeTracking;

namespace Template.Infrastructure
{
    public class DataGenerator
    {
        public static void InitializeUsers(TemplateDbContext context)
        {
            if (context.Users.Any())
            {
                return;   // Data was already seeded
            }

            context.Users.AddRange(
                new User
                {
                    Id = Guid.Parse("c5a1c8d0-4f2d-4e20-9f54-2c6c9f7b8b77"),
                    Email = "admin@test.it",
                    Password = "M0Cuk9OsrcS/rTLGf5SY6DUPqU2rGc1wwV2IL88GVGo=", // SHA-256 of text "Prova"
                    FirstName = "Admin",
                    LastName = "Master",
                    NickName = "NickNameAdmin",
                    Role = "Admin"
                },
                new User
                {
                    Id = Guid.Parse("9f3e2b59-0c74-4b03-8a46-4c5d9fa1a221"),
                    Email = "teamleader@test.it",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Leader",
                    LastName = "Team",
                    NickName = "NicknameLeader",
                    Role = "TeamLeader"
                },
                new User
                {
                    Id = Guid.Parse("3de6883f-9a0b-4667-aa53-0fbc52c4d300"), // Forced to specific Guid for tests
                    Email = "email1@test.it",
                    Password = "M0Cuk9OsrcS/rTLGf5SY6DUPqU2rGc1wwV2IL88GVGo=", // SHA-256 of text "Prova"
                    FirstName = "Nome1",
                    LastName = "Cognome1",
                    NickName = "Nickname1",
                    Role = "User"
                },
                new User
                {
                    Id = Guid.Parse("a030ee81-31c7-47d0-9309-408cb5ac0ac7"), // Forced to specific Guid for tests
                    Email = "email2@test.it",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Nome2",
                    LastName = "Cognome2",
                    NickName = "Nickname2",
                    Role = "User"
                },
                new User
                {
                    Id = Guid.Parse("bfdef48b-c7ea-4227-8333-c635af267354"), // Forced to specific Guid for tests
                    Email = "email3@test.it",
                    Password = "Uy6qvZV0iA2/drm4zACDLCCm7BE9aCKZVQ16bg80XiU=", // SHA-256 of text "Test"
                    FirstName = "Nome3",
                    LastName = "Cognome3",
                    NickName = "Nickname3",
                    Role = "User"
                });

            context.SaveChanges();
        }

        public static void InitializeTimeTracking(TemplateDbContext context)
        {
            if (context.Projects.Any()) return;

            var project1 = new Project { Name = "Commessa A" };
            var project2 = new Project { Name = "Commessa B" };

            context.Projects.AddRange(project1, project2);

            var user = context.Users.First();

            context.TimeEntries.AddRange(
                new TimeEntry
                {
                    UserId = user.Id,
                    Date = DateTime.Today.AddDays(-1),
                    HoursWorked = 8,
                    ProjectId = project1.Id,
                    Notes = "Analisi requisiti"
                },
                new TimeEntry
                {
                    UserId = user.Id,
                    Date = DateTime.Today,
                    HoursWorked = 6,
                    ProjectId = project2.Id,
                    Notes = "Sviluppo funzionalità"
                }
            );

            context.SaveChanges();
        }
    }
}
