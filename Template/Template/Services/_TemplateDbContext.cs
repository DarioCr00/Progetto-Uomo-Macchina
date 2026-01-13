using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Template.Infrastructure;
using Template.Services.Shared;
using Template.Services.Shared.TimeTracking;

namespace Template.Services
{
    public class TemplateDbContext : DbContext
    {
        public TemplateDbContext()
        {
        }

        public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
        {
            //DataGenerator.InitializeUsers(this);

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique(false);
            modelBuilder.Entity<TimeEntry>().HasIndex(t => new { t.UserId, t.Date });
            modelBuilder.Entity<Project>().HasIndex(p => p.Code).IsUnique();
            modelBuilder.Entity<TaskAssignment>().HasKey(ta => new { ta.TaskId, ta.UserId });

            modelBuilder.Entity<Project>()
                .Property(p => p.CreatedByUserId)
                .ValueGeneratedNever()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.CreatedByUserId)
                .ValueGeneratedNever()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
