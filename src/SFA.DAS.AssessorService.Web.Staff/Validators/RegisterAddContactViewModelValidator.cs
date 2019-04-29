using System;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class RegisterAddContactViewModelValidator : AbstractValidator<RegisterAddContactViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;

       public RegisterAddContactViewModelValidator(IOrganisationsApiClient apiClient)
        {
            _apiClient = apiClient;
          
            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult =  _apiClient.ValidateCreateContact(vm.FirstName, vm.LastName, vm.EndPointAssessorOrganisationId, vm.Email, vm.PhoneNumber).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}