namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    using System;
    using System.Collections.Generic;
    using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

    public class AddOrganisationValidator : IAddOrganisationValidator
    {
        public IEnumerable<string> ValidateProviderType(int providerTypeId)
        {
            var isValid = (providerTypeId >= 1 && providerTypeId <= 3);

            if (!isValid)
            {
                return new List<string>
                {
                    AddOrganisationValidationErrorMessages.InvalidProviderTypeId
                };
            } 

            return new List<string>();
        }

        public IEnumerable<string> ValidateOrganisationDetails(AddOrganisationViewModel viewModel)
        {
            var validationErrorMessages = new List<string>();

            if (String.IsNullOrWhiteSpace(viewModel.LegalName))
            {
                validationErrorMessages.Add(AddOrganisationValidationErrorMessages.LegalNameIsMandatory);
            }

            if (String.IsNullOrWhiteSpace(viewModel.UKPRN))
            {
                validationErrorMessages.Add(AddOrganisationValidationErrorMessages.UKPRNIsMandatory);
            }

            return validationErrorMessages;
        }

    }
}
