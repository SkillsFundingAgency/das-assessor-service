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
    public class GetAssessmentOrganisationHandler : IRequestHandler<GetAssessmentOrganisationRequest, AssessmentOrganisationDetails>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAssessmentOrganisationHandler> _logger;

        public GetAssessmentOrganisationHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAssessmentOrganisationHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<AssessmentOrganisationDetails> Handle(GetAssessmentOrganisationRequest request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;
            _logger.LogInformation($@"Handling AssessmentOrganisation Request for [{organisationId}]");
            var org = await _registerQueryRepository.GetAssessmentOrganisation(organisationId);

            if (org == null) return null;

            var addresses = await _registerQueryRepository.GetAssessmentOrganisationAddresses(organisationId);
            org.Address = addresses.FirstOrDefault();

            var contact = await GetPrimaryOrFirstContact(organisationId);

            if (contact == null) return org;

            org.Email = contact.Email;
            org.Phone = contact.PhoneNumber;
            return org;
        }

        private async Task<AssessmentOrganisationContact> GetPrimaryOrFirstContact(string organisationId)
        {
            var contacts = await _registerQueryRepository.GetAssessmentOrganisationContacts(organisationId);
            var assessmentOrganisationContacts = contacts as IList<AssessmentOrganisationContact> ?? contacts.ToList();
            var contact = assessmentOrganisationContacts.Any(x => x.IsPrimaryContact) 
                    ? assessmentOrganisationContacts.First(x => x.IsPrimaryContact) 
                    : assessmentOrganisationContacts.FirstOrDefault();

            return contact;
        }
    }
}
