using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class CheckIfOfsOrganisationHandler : IRequestHandler<CheckIfOfsOrganisationRequest, bool>
    {
        private readonly ILogger<CheckIfOfsOrganisationHandler> _logger;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public CheckIfOfsOrganisationHandler(ILogger<CheckIfOfsOrganisationHandler> logger, IOrganisationQueryRepository organisationQueryRepository)
        {
            _logger = logger;
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<bool> Handle(CheckIfOfsOrganisationRequest request, CancellationToken cancellationToken)
        {
            return await _organisationQueryRepository.IsOfsOrganisation(request.Ukprn);
        }
    }
}
