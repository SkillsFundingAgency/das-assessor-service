namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    using System.Collections.Generic;
    using ViewModels.Roatp;

    public interface IAddOrganisationValidator
    {
        IEnumerable<string> ValidateProviderType(int providerTypeId);
        IEnumerable<string> ValidateOrganisationDetails(AddOrganisationViewModel viewModel);
    }
}
