using System;

namespace ProgressTrackerModel
{
    public class SprintCQMTrackerModel
    {
        public int CmsId { get; set; }
        public string ClientType { get; set; }
        public string DevOwner { get; set; }
        public DateTime? DevStartDate { get; set; }
        public DateTime? DevEndDate { get; set; }
        public int DevDays { get; set; }
        public int GeneratorDevProgress { get; set; }
        public string CustomDevOwner { get; set; }
        public DateTime? CustomCodeStartDate { get; set; }
        public DateTime? CustomDevEndDate { get; set; }
        public int CustomDevDays { get; set; }
        public int CustomDevProgress { get; set; }

        public string QaOwner { get; set; }
        public DateTime? QaStartDate { get; set; }
        public DateTime? QaEndDate { get; set; }

        public int QaDays { get; set; }

        public int QaProgress { get; set; }

        public bool IsStoryDone { get; set; }

        /// <summary>
        /// automation completion percent
        /// </summary>
        public int AutomationDone { get; set; }

        /// <summary>
        /// test planning status.
        /// </summary>
        public int TestPlanningDone { get; set; }

        public bool IsReviewed { get; set; }

        public string Remarks { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }


           //,<QaOwner, varchar(50),>
           //,<QaStartDev, smalldatetime,>
           //,<QaEndDate, smalldatetime,>
           //,<QaDays, int,>
           //,<QaProgress, int,>
           //,<IsDone, bit,>
           //,<Automatuion, int,>
           //,<ManualTest, int,>
           //,<IsReviewed, bit,>
           //,<Remarks, varchar(500),>
           //,<CreatedDate, smalldatetime,>
           //,<UpdatedDate, smalldatetime,>)
    }
}
