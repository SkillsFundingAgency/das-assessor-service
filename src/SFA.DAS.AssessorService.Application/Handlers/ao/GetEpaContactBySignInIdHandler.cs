using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetEpaContactBySignInIdHandler : IRequestHandler<GetEpaContactBySignInIdRequest, EpaContact>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetEpaContactBySignInIdHandler> _logger;

        public GetEpaContactBySignInIdHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetEpaContactBySignInIdHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<EpaContact> Handle(GetEpaContactBySignInIdRequest request, CancellationToken cancellationToken)
        {
            var signInId = request.SignInId;
            _logger.LogInformation($@"Handling Get EpaContact Request for SignInId [{signInId}]");

            var contact = await _registerQueryRepository.GetContactBySignInId(signInId);

            return contact ?? null;
        }
    }
}
