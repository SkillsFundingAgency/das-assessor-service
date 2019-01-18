using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Applications
{
    public class ApplicationSectionViewModel
    {
        public ApplicationSection Section { get; }

        public string Title { get; }

        public Guid ApplicationId { get; }

        public int SequenceId { get; }

        public int SectionId { get; }

        public bool? IsSectionComplete { get; set; }

        public ApplicationSectionViewModel(ApplicationSection section)
        {
            Section = section;
            Title = section.Title;
            ApplicationId = section.ApplicationId;
            SequenceId = section.SequenceId;
            SectionId = section.SectionId;

            if (section.Status == ApplicationSectionStatus.Evaluated)
            {
                IsSectionComplete = true;
            }
        }
    }
}
