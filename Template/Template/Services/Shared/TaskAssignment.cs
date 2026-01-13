using System;
using Template.Services.Shared.TimeTracking;

namespace Template.Services.Shared
{
    public class TaskAssignment
    {
        public Guid TaskId { get; set; }
        public TaskItem Task { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
