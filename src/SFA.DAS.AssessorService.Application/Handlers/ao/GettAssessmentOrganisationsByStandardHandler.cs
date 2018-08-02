using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GettAssessmentOrganisationsByStandardHandler : IRequestHandler<GetAssessmentOrganisationsbyStandardRequest, List<AssessmentOrganisationDetails>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAssessmentOrganisationHandler> _logger;

        public GettAssessmentOrganisationsByStandardHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAssessmentOrganisationHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<List<AssessmentOrganisationDetails>> Handle(GetAssessmentOrganisationsbyStandardRequest request, CancellationToken cancellationToken)
        {
            var standardId = request.StandardId;
            _logger.LogInformation($@"Handling AssessmentOrganisations Request for StandardId [{standardId}]");
            var organisations = await _registerQueryRepository.GetAssessmentOrganisationsByStandardId(request.StandardId);

           foreach (var org in organisations)
            {
                var addresses = await _registerQueryRepository.GetAssessmentOrganisationAddresses(org.Id);
                org.Address = addresses.FirstOrDefault();
                var contact = await _registerQueryRepository.GetPrimaryOrFirstContact(org.Id);
                if (contact == null) continue;
                org.Email = contact.Email;
                org.Phone = contact.PhoneNumber;
            }

            return organisations.ToList();
        }

    }
}

