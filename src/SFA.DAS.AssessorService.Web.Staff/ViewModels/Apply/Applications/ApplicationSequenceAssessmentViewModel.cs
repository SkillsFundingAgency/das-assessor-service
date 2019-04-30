﻿using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Applications
{
    public class ApplicationSequenceAssessmentViewModel
    {
        public ApplicationSequence Sequence { get; }

        public string Title { get; }

        public Guid ApplicationId { get; }

        public int SequenceId { get; }

        public bool HasNewFeedback { get; }

        public bool HasInadequateFhaButNoFeedbackGiven { get; }

        public string ReturnType { get; set; }

        public ApplicationSequenceAssessmentViewModel(ApplicationSequence sequence)
        {
            Sequence = sequence;
            Title = "Assessment summary";
            ApplicationId = sequence.ApplicationId;
            SequenceId = sequence.SequenceId;
            HasNewFeedback = sequence.Sections.Any(s => s.HasNewPageFeedback);

            var fhaSection = sequence.Sections.Where(s => s.SectionId == 3).FirstOrDefault();

            if(fhaSection != null && !fhaSection.HasNewPageFeedback)
            {
                HasInadequateFhaButNoFeedbackGiven = fhaSection.QnAData?.FinancialApplicationGrade?.SelectedGrade == FinancialApplicationSelectedGrade.Inadequate;
            }
        }
    }
}
