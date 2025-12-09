using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Services.Shared.TimeTracking.DTOs
{
    public class TimeSummaryFilterDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        //opzionale: filtrare un singolo progetto
        public Guid? ProjectId { get; set; }
    }
}
