namespace SFA.DAS.AssessorService.Web.Staff.Validators.Roatp
{
    using System.Threading.Tasks;
    using Api.Types.Models.Validation;
    using ViewModels.Roatp;

    public interface IAddOrganisationValidator
    {
        Task<ValidationResponse> ValidateOrganisationDetails(AddOrganisationViewModel viewModel);
    }
}
