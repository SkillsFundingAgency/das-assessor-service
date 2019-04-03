using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Linq;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Apply.Applications
{
    public class ApplicationSequenceViewModel
    {
        public string ApplicationReference { get; }
        public string LegalName { get; }
        public string TradingName { get; }
        public string ProviderName { get; }
        public int? Ukprn { get; }
        public string CompanyNumber { get; }
        public DateTime? FinancialDueDate { get; }

        public ApplicationSequence Sequence { get; }

        public Guid ApplicationId { get; }

        public int SequenceId { get; }


        public ApplicationSequenceViewModel(Guid applicationId, int sequenceId, ApplicationSequence sequence, ApplyTypes.Application application)
        {
            if (sequence != null)
            {
                Sequence = sequence;
                ApplicationId = sequence.ApplicationId;
                SequenceId = sequence.SequenceId;
            }
            else
            {
                ApplicationId = applicationId;
                SequenceId = sequenceId;
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

                    if (!sequence.Sections.All(s => s.SectionId != 3))
                    {
                        FinancialDueDate = application.ApplyingOrganisation.OrganisationDetails.FHADetails?.FinancialDueDate;
                    }
                }
            }
        }
    }
}
