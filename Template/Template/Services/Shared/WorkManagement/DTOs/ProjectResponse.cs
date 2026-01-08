using System;
using System.Collections.Generic;

namespace Template.Services.Shared.WorkManagement.DTOs
{
    public class ProjectWithTasksDto
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
        public string CreatedByUserGravatar { get; set; }

        public List<TaskDto> Tasks { get; set; } = new();
    }

    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public Guid ProjectId { get; set; }

        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
        public string CreatedByUserGravatar { get; set; }

        public List<UserDto> AssignedUsers { get; set; } = new();
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
