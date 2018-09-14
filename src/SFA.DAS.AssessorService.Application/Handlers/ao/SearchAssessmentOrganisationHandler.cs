using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class SearchAssessmentOrganisationHandler : IRequestHandler<SearchAssessmentOrganisationsRequest, List<AssessmentOrganisationSummary>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<SearchAssessmentOrganisationHandler> _logger;
        private readonly IEpaOrganisationSearchValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public SearchAssessmentOrganisationHandler(IRegisterQueryRepository registerQueryRepository, IEpaOrganisationSearchValidator validator, ILogger<SearchAssessmentOrganisationHandler> logger, ISpecialCharacterCleanserService cleanser)
        {
            _registerQueryRepository = registerQueryRepository;
            _validator = validator;
            _logger = logger;
            _cleanser = cleanser;
        }

        public async Task<List<AssessmentOrganisationSummary>> Handle(SearchAssessmentOrganisationsRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling Search AssessmentOrganisations Request");
            
            var searchstring = _cleanser.CleanseStringForSpecialCharacters(request.Searchstring);

            if (searchstring.Length < 2)
                throw new BadRequestException("The searchstring is too short to do a valid search");

            if (_validator.IsValidEpaOrganisationId(searchstring))
            {
                _logger.LogInformation($@"Searching AssessmentOrganisations based on organisationId: [{searchstring}]");
                var result = await _registerQueryRepository.GetAssessmentOrganisationsByOrganisationId(searchstring);
                return result.ToList();
            }

            if (_validator.IsValidUkprn(searchstring))
            {
                _logger.LogInformation($@"Searching AssessmentOrganisations based on ukprn: [{searchstring}]");
                var result = await _registerQueryRepository.GetAssessmentOrganisationsByUkprn(searchstring);
                return result.ToList();
            }

            _logger.LogInformation($@"Searching AssessmentOrganisations based on name wildcard: [{searchstring}]");
            var resultMain = await _registerQueryRepository.GetAssessmentOrganisationsbyName(searchstring);
            return resultMain.ToList();
        }
    }
}