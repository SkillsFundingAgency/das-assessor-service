namespace SFA.DAS.AssessorService.Application.Api.Orchestrators
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using Consts;
    using Exceptions;
    using Interfaces;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Validators;

    public class GetOrganisationsOrchestrator
    {
        private readonly IStringLocalizer<GetOrganisationsOrchestrator> _localizer;
        private readonly ILogger<GetOrganisationsOrchestrator> _logger;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly UkPrnValidator _ukPrnValidator;

        public GetOrganisationsOrchestrator(IOrganisationQueryRepository organisationQueryRepository,
            IStringLocalizer<GetOrganisationsOrchestrator> localizer,
            UkPrnValidator ukPrnValidator,
            ILogger<GetOrganisationsOrchestrator> logger)
        {
            _organisationQueryRepository = organisationQueryRepository;
            _localizer = localizer;
            _ukPrnValidator = ukPrnValidator;
            _logger = logger;
        }

        public async Task<Organisation> GetOrganisation(int ukprn)
        {
            var result = _ukPrnValidator.Validate(ukprn);
            if (!result.IsValid)
                throw new ApplicationException(result.Errors[0].ErrorMessage);

            var organisation = await _organisationQueryRepository.GetByUkPrn(ukprn);
            if (organisation == null)
                throw new ResourceNotFoundException(_localizer[ResourceMessageName.NoAssesmentProviderFound, ukprn]
                    .Value);

            return organisation;
        }

        public async Task<IEnumerable<Organisation>> GetOrganisations()
        {
            var organisations = await _organisationQueryRepository.GetAllOrganisations();
            return organisations;
        }
    }
}