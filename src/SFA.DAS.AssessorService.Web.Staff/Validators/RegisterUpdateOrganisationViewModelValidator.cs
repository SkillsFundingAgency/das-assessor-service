﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Staff.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Validators
{   
    public class RegisterUpdateOrganisationViewModelValidator : AbstractValidator<RegisterViewAndEditOrganisationViewModel>
    {
        private readonly IOrganisationsApiClient _apiClient;

        public RegisterUpdateOrganisationViewModelValidator(IOrganisationsApiClient apiClient)
        {
            _apiClient = apiClient;

            RuleFor(vm => vm).Custom((vm, context) =>
            {
                var validationResult = _apiClient.ValidateUpdateOrganisation(vm.OrganisationId, vm.Name, vm.Ukprn?.ToString(), vm.OrganisationTypeId?.ToString()).Result;
                if (validationResult.IsValid) return;
                foreach (var error in validationResult.Errors)
                {
                    context.AddFailure(error.Field, error.ErrorMessage);
                }
            });
        }
    }
}
