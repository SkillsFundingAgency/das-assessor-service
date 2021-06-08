using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetStandardVersionsByOrganisationIdAndStandardReferenceHandler : IRequestHandler<GetStandardVersionsByOrganisationIdAndStandardReferenceRequest, IEnumerable<AppliedStandardVersion>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetStandardVersionsByOrganisationIdAndStandardReferenceHandler> _logger;

        public GetStandardVersionsByOrganisationIdAndStandardReferenceHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetStandardVersionsByOrganisationIdAndStandardReferenceHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AppliedStandardVersion>> Handle(GetStandardVersionsByOrganisationIdAndStandardReferenceRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling GetStandardVersionsByOrganisationIdAndStandardReference Request for OrganisationId [{request.OrganisationId}] and Standard Reference[{request.StandardReference}]");
            
            return await _registerQueryRepository.GetStandardVersionsByOrganisationIdAndStandardReference(request.OrganisationId, request.StandardReference);
        }
    } 
}
