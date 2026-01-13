using System;
using System.Collections.Generic;

namespace Template.Services.Shared.WorkManagement.DTOs
{
    public class CreateTaskRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid ProjectId { get; set; }
        public List<Guid> AssignedUserIds { get; set; } = new();
    }

    public class EditTaskRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid ProjectId { get; set; }
        public List<Guid> AssignedUserIds { get; set; } = new();
    }
}
