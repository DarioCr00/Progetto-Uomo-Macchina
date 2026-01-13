using System;

namespace Template.Services.Shared.WorkManagement.DTOs
{
    public class CreateProjectRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class EditProjectRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    };
}
