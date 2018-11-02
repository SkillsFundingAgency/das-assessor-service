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
                var searchstring = vm.StandardSearchString?.Trim().ToLower();
                searchstring = string.IsNullOrEmpty(searchstring) ? "" : searchstring;
                var rx = new System.Text.RegularExpressions.Regex("<[^>]*>/");
                searchstring = rx.Replace(searchstring, "");
                searchstring = searchstring.Replace("/", "");
                var searchTerm = Uri.EscapeUriString(searchstring);
                var validationResult = _apiClient.ValidateSearchStandards(searchTerm).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
