using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetContactByEmailHandler : IRequestHandler<GetContactByEmailRequest, EpaContact>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetContactByEmailHandler> _logger;

        public GetContactByEmailHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetContactByEmailHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<EpaContact> Handle(GetContactByEmailRequest request, CancellationToken cancellationToken)
        {
            var email = request.Email;
            _logger.LogInformation($@"Handling Get Contact Request for email [{email}]");

            var contact = await _registerQueryRepository.GetContactByEmail(email);

            return contact ?? null;
        }
    }
}
