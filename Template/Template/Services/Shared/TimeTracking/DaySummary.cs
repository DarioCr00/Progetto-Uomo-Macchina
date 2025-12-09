using System;

namespace Template.Services.Shared.TimeTracking
{
    public class DaySummary
    {
        public DateTime Date { get; set; }
        public decimal TotalHours { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsWeekend { get; set; }
    }
}
