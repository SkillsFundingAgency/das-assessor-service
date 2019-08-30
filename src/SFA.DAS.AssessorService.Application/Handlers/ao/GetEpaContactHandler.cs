using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetEpaContactHandler : IRequestHandler<GetEpaContactRequest, EpaContact>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetEpaContactHandler> _logger;

        public GetEpaContactHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetEpaContactHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<EpaContact> Handle(GetEpaContactRequest request, CancellationToken cancellationToken)
        {
            var contactId = request.ContactId;
            _logger.LogInformation($@"Handling Get EpaContact Request for ContactId [{contactId}]");

            var contact = await _registerQueryRepository.GetContactByContactId(contactId);

            return contact ?? null;
        }
    }
}
