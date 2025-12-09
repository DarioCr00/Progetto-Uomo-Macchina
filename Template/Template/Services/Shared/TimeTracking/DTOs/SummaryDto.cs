using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Services.Shared.TimeTracking.DTOs
{
    public class TimeSummaryDto
    {
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<TagSummaryDto> Tags { get; set; }
        public decimal TotalHours { get; set; }
    }

    public class TagSummaryDto
    {
        public Guid TagId { get; set; }
        public string TagName { get; set; }
        public decimal Hours { get; set; }
    }
}
