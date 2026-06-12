using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetEpaContactByGovUkIdentifierHandler : IRequestHandler<GetEpaContactByGovUkIdentifierRequest, EpaContact>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetEpaContactByGovUkIdentifierHandler> _logger;

        public GetEpaContactByGovUkIdentifierHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetEpaContactByGovUkIdentifierHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<EpaContact> Handle(GetEpaContactByGovUkIdentifierRequest request, CancellationToken cancellationToken)
        {
            var govUkIdentifier = request.GovUkIdentifier;
            _logger.LogInformation($@"Handling Get EpaContact Request for GovUkIdentifier [{govUkIdentifier}]");

            var contact = await _registerQueryRepository.GetContactByGovUkIdentifier(govUkIdentifier);

            return contact ?? null;
        }
    }
}
