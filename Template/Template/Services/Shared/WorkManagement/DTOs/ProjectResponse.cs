using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Services.Shared.WorkManagement.DTOs
{
    public class ProjectWithTasksDto
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public List<TaskDto> Tasks { get; set; } = new();
    }

    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
