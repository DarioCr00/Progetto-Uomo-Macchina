using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Template.Services.Shared.TimeTracking
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        public Guid CreatedByUserId { get; set; }

        public List<TaskItem> Tasks { get; set; } = new();
        public ICollection<TimeEntry> TimeEntries { get; set; }
    }
}
