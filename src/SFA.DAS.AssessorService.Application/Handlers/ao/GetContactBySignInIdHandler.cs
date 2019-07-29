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
    public class GetContactBySignInIdHandler : IRequestHandler<GetContactBySignInIdRequest, EpaContact>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetContactBySignInIdHandler> _logger;

        public GetContactBySignInIdHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetContactBySignInIdHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<EpaContact> Handle(GetContactBySignInIdRequest request, CancellationToken cancellationToken)
        {
            var signInId = request.SignInId;
            _logger.LogInformation($@"Handling Get Contact Request for SignInId [{signInId}]");

            var contact = await _registerQueryRepository.GetContactBySignInId(signInId);

            return contact ?? null;
        }
    }
}
