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

        public ApplicationSectionViewModel(Guid applicationId, int sequenceId,  int sectionId, ApplicationSection section)
        {
            if (section != null)
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
            else
            {
                ApplicationId = applicationId;
                SequenceId = sequenceId;
                SectionId = sectionId;
            }
        }
    }
}
