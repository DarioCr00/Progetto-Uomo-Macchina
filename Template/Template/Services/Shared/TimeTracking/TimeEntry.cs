using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Services.Shared.TimeTracking
{
    public enum WorkType { Normal = 0, Overtime = 1, Travel = 2, Other = 3 }

    public class TimeEntry
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Range(0, 24)] 
        public decimal HoursWorked { get; set; } //in una giornata

        public WorkType Type { get; set; } = WorkType.Normal;

        public Guid? TaskId { get; set; }

        [ForeignKey(nameof(TaskId))]
        public TaskItem Task { get; set; }

        public Guid? ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        public List<Tag> Tags { get; set; } = new();

        public string Notes { get; set; }
    }
}
