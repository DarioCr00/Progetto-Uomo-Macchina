namespace Template.Services.Shared.TimeTracking.DTOs
{
    public class TimeEntryUpdateDto
    {
        public decimal HoursWorked { get; set; }
        public string Notes { get; set; }
        public WorkType Type { get; set; }
    }
}
