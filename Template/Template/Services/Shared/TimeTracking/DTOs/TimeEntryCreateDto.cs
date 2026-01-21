using System;

namespace Template.Services.Shared.TimeTracking.DTOs
{
    public class TimeEntryCreateDto
    {
        public Guid? ProjectId { get; set; }
        public Guid? TaskId { get; set; }
        public DateTime Date { get; set; }
        public decimal HoursWorked { get; set; }
        public string Notes { get; set; }
        public WorkType Type { get; set; }
    }
}
