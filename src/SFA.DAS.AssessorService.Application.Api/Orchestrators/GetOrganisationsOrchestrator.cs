using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Consts;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.Orchestrators
{
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

        public async Task<OrganisationResponse> SearchOrganisation(int ukprn)
        {
            var result = _ukPrnValidator.Validate(ukprn);
            if (!result.IsValid)
                throw new BadRequestException(result.Errors[0].ErrorMessage);

            var organisation = await _organisationQueryRepository.GetByUkPrn(ukprn);
            if (organisation == null)
            {
                var ex = new ResourceNotFoundException(
                    _localizer[ResourceMessageName.NoAssesmentProviderFound, ukprn]
                        .Value);
                throw ex;
            }

            return organisation;
        }

        public async Task<IEnumerable<OrganisationResponse>> GetOrganisations()
        {
            var organisations = await _organisationQueryRepository.GetAllOrganisations();
            return organisations;
        }
    }
}