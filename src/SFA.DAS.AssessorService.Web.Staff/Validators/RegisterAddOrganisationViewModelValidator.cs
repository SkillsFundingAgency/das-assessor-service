using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class RegisterAddOrganisationViewModelValidator : AbstractValidator<RegisterOrganisationViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;

        public RegisterAddOrganisationViewModelValidator(IOrganisationsApiClient apiClient)
        {
            _apiClient = apiClient;
          
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult =  _apiClient.ValidateCreateOrganisation(vm.Name, vm.Ukprn?.ToString(), vm.OrganisationTypeId?.ToString()).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
