using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetContactHandler : IRequestHandler<GetContactRequest, AssessmentOrganisationContact>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetContactHandler> _logger;

        public GetContactHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetContactHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<AssessmentOrganisationContact> Handle(GetContactRequest request, CancellationToken cancellationToken)
        {
            var contactId = request.ContactId;
            _logger.LogInformation($@"Handling Get Contact Request for [{contactId}]");
            var contact = await _registerQueryRepository.GetAssessmentOrganisationContact(contactId);

            return contact ?? null;
        }  
    }
}
