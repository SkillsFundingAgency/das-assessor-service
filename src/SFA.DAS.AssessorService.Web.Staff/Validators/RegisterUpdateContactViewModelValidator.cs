using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class RegisterUpdateContactViewModelValidator: AbstractValidator<RegisterViewAndEditContactViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;
       
        public RegisterUpdateContactViewModelValidator(IOrganisationsApiClient apiClient)
        {
            _apiClient = apiClient;
        RuleFor(vm => vm).Custom((vm, context) =>
        {
            var validationResult =  _apiClient.ValidateUpdateContact(vm.ContactId, vm.DisplayName, vm.Email).Result;
            if (validationResult.IsValid) return;
            foreach (var error in validationResult.Errors)
            {
                context.AddFailure(error.Field, error.ErrorMessage);
            }
        });
    }
}}