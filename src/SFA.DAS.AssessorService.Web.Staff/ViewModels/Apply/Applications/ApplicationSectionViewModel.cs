using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Applications
{
    public class ApplicationSectionViewModel
    {
        public string ApplicationReference { get; }
        public string LegalName { get; }
        public string TradingName { get; }
        public string ProviderName { get; }
        public int? Ukprn { get; }
        public string CompanyNumber { get; }

        public ApplicationSection Section { get; }

        public string Title { get; }

        public Guid ApplicationId { get; }

        public int SequenceId { get; }

        public int SectionId { get; }

        public bool? IsSectionComplete { get; set; }

        public ApplicationSectionViewModel(Guid applicationId, int sequenceId,  int sectionId, ApplicationSection section, ApplyTypes.Application application)
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

            if (application != null)
            {
                if (application.ApplicationData != null)
                {
                    ApplicationReference = application.ApplicationData.ReferenceNumber;
                }

                if (application.ApplyingOrganisation?.OrganisationDetails != null)
                {
                    Ukprn = application.ApplyingOrganisation.OrganisationUkprn;
                    LegalName = application.ApplyingOrganisation.OrganisationDetails.LegalName;
                    TradingName = application.ApplyingOrganisation.OrganisationDetails.TradingName;
                    ProviderName = application.ApplyingOrganisation.OrganisationDetails.ProviderName;
                    CompanyNumber = application.ApplyingOrganisation.OrganisationDetails.CompanyNumber;
                }
            }
        }
    }
}
