using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetEpaContactByEmailHandler : IRequestHandler<GetEpaContactByEmailRequest, EpaContact>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetEpaContactByEmailHandler> _logger;

        public GetEpaContactByEmailHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetEpaContactByEmailHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<EpaContact> Handle(GetEpaContactByEmailRequest request, CancellationToken cancellationToken)
        {
            var email = request.Email;
            _logger.LogInformation($@"Handling Get EpaContact Request for email [{email}]");

            var contact = await _registerQueryRepository.GetContactByEmail(email);

            return contact ?? null;
        }
    }
}
