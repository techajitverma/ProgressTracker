using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgressTrackerModel
{
  public class SprintStoryTrackerModel
    {
        public int SprintStoryTrackerId { get; set; }
        public int SprintNumber { get; set; }
        public string StoryNumber { get; set; }
        public string StoryDescription { get; set; }
        public bool IsCurrentSprintStory { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal TotalHours { get; set; }
        public int ContributionPercent { get; set; }
        public string Remarks { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IActive { get; set; }


    }
}
