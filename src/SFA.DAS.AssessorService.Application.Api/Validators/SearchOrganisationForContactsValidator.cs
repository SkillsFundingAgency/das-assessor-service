using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Validators
{
    public class SearchOrganisationForContactsValidator
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IStringLocalizer<SearchOrganisationForContactsValidator> _localiser;

        public SearchOrganisationForContactsValidator(IOrganisationQueryRepository organisationQueryRepository,
            IStringLocalizer<SearchOrganisationForContactsValidator> localiser)
        {
            _organisationQueryRepository = organisationQueryRepository;
            _localiser = localiser;
        }

        public ValidationResult Validate(string endPointAssessorOrganisationId)
        {
            var validationResult = new ValidationResult();

            var isValid = _organisationQueryRepository.CheckIfAlreadyExists(endPointAssessorOrganisationId).Result;
            if (isValid)
                validationResult = new ValidationResult();
            else
                validationResult.Errors.Add(new ValidationFailure("Organisation",                  
                    string.Format(_localiser[ResourceMessageName.DoesNotExist].Value, "Organisation", endPointAssessorOrganisationId)));

            return validationResult;            
        }
    }
}


