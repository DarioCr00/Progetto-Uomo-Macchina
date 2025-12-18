using System;
using System.Collections.Generic;

namespace Template.Services.Shared.TimeTracking.DTOs
{
    public class TimeEntryDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public DateTime Date { get; set; }
        public decimal HoursWorked { get; set; }
        public string Notes { get; set; }
        public WorkType Type { get; set; }
        public List<string> Tags { get; set; }
    }
}
