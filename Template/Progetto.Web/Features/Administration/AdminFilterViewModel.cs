using System;
using System.Collections.Generic;
using Template.Services.Shared;
using Template.Services.Shared.TimeTracking;

namespace Progetto.Web.Features.Administration
{
    public class AdminFilterViewModel
    {
        public IEnumerable<UsersIndexDTO.User> Users { get; set; } = new List<UsersIndexDTO.User>();
        public IEnumerable<Project> Projects { get; set; } = new List<Project>();
        public IEnumerable<TaskItem> Tasks { get; set; } = new List<TaskItem>();

        //filtri selezionati
        public Guid? SelectedUserId { get; set; }
        public Guid? SelectedProjectId { get; set; }
        public Guid? SelectedTaskId { get; set; }
        public string SearchText { get; set; }
    }
}
