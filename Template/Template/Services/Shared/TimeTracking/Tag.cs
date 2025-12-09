using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Template.Services.Shared.TimeTracking
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public List<TimeEntry> TimeEntries { get; set; } = new();
    }
}
