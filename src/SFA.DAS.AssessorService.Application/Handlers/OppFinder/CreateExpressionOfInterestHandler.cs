using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class CreateExpressionOfInterestHandler : IRequestHandler<OppFinderExpressionOfInterestRequest, bool>
    {
        private readonly ILogger<CreateExpressionOfInterestHandler> _logger;
        private readonly IOppFinderRepository _oppFinderRepository;

        public CreateExpressionOfInterestHandler(ILogger<CreateExpressionOfInterestHandler> logger, IOppFinderRepository oppFinderRepository)
        {
            _logger = logger;
            _oppFinderRepository = oppFinderRepository;
        }

        public async Task<bool> Handle(OppFinderExpressionOfInterestRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Creating expression of interest");
                return await _oppFinderRepository.CreateExpressionOfInterest(request.StandardReference, request.Email, request.OrganisationName, request.ContactName, request.ContactNumber);
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"Unable to create expression of interest for {request.StandardReference}, {request.Email}, {request.OrganisationName}, {request.ContactName}, {request.ContactNumber}");
                throw;
            }
        }
    }
}
