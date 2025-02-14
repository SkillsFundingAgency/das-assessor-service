﻿using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class ApprovalsExtractModel : TestModel
    {
        public int? ApprenticeshipId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ULN { get; set; }

        public int? TrainingCode { get; set; }

        public string TrainingCourseVersion { get; set; }

        public bool TrainingCourseVersionConfirmed { get; set; }

        public string TrainingCourseOption { get; set; }

        public string StandardUId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public DateTime? StopDate { get; set; }

        public DateTime? PauseDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public int? UKPRN { get; set; }

        public string LearnRefNumber { get; set; }

        public int? PaymentStatus { get; set; }

        public long? EmployerAccountId { get; set; }
        public string EmployerName { get; set; }
    }
}
