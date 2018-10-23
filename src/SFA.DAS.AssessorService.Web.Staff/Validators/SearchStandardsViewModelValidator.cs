using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{
    public class SearchStandardsViewModelValidator : AbstractValidator<SearchStandardsViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;

        public SearchStandardsViewModelValidator(IOrganisationsApiClient apiClient)
        {
            _apiClient = apiClient;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = _apiClient.ValidateSearchStandards(vm.StandardSearchString).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
