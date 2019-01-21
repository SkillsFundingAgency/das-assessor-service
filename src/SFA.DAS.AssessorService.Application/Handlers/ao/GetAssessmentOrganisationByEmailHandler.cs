using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class GetAssessmentOrganisationByEmailHandler : IRequestHandler<GetAssessmentOrganisationByEmailRequest, AssessmentOrganisationSummary>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAssessmentOrganisationByEmailRequest> _logger;
        private readonly IEpaOrganisationValidator _validator;
        private readonly ISpecialCharacterCleanserService _cleanser;

        public GetAssessmentOrganisationByEmailHandler(IRegisterQueryRepository registerQueryRepository, IEpaOrganisationValidator validator, ILogger<GetAssessmentOrganisationByEmailRequest> logger, ISpecialCharacterCleanserService cleanser)
        {
            _registerQueryRepository = registerQueryRepository;
            _validator = validator;
            _logger = logger;
            _cleanser = cleanser;
        }

        public async Task<AssessmentOrganisationSummary> Handle(GetAssessmentOrganisationByEmailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling Search AssessmentOrganisations Request");

            var email = _cleanser.CleanseStringForSpecialCharacters(request.Email.Trim());


            if (_validator.CheckIfEmailIsPresentAndInSuitableFormat(email) != string.Empty)
            {
                _logger.LogInformation($@"Getting AssessmentOrganisation based on contact email with invalid email address format: [{email}]");
                return (AssessmentOrganisationSummary) null;
            }

            _logger.LogInformation($@"Getting AssessmentOrganisation based on contact email: [{email}]");
            var result = await _registerQueryRepository.GetAssessmentOrganisationByContactEmail(email);
            return result;
        }
    }
}

