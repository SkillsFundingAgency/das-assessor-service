namespace SFA.DAS.AssessorService.Application.Api.Orchesrators
{
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Api.Types;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Api.Validators;
    using SFA.DAS.AssessorService.Application.Exceptions;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class GetOrganisationsOrchestrator
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IStringLocalizer<GetOrganisationsOrchestrator> _localizer;
        private readonly UkPrnValidator _ukPrnValidator;
        private readonly ILogger<GetOrganisationsOrchestrator> _logger;

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
            {
                throw new ResourceNotFoundException(_localizer[ResourceMessageName.NoAssesmentProviderFound, ukprn].Value);
            }

            return organisation;
        }

        public async Task<IEnumerable<Organisation>> GetOrganisations()
        {
            var organisations = await _organisationQueryRepository.GetAllOrganisations();
            return organisations;
        }
    }
}


