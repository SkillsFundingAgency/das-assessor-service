using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class InvitationCheckHandler : IRequestHandler<InvitationCheckRequest>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMediator _mediator;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;

        public InvitationCheckHandler(
            IContactRepository contactRepository,
            IMediator mediator,
            IContactQueryRepository contactQueryRepository,
            IOrganisationQueryRepository organisationQueryRepository,
            IEMailTemplateQueryRepository eMailTemplateQueryRepository
        )
        {
            _contactRepository = contactRepository;
            _mediator = mediator;
            _contactQueryRepository = contactQueryRepository;
            _organisationQueryRepository = organisationQueryRepository;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
        }
        public async Task<Unit> Handle(InvitationCheckRequest message, CancellationToken cancellationToken)
        {
            var contactInvitation = await _contactRepository.GetContactInvitation(message.ContactId);
            if (contactInvitation is null || contactInvitation.IsAccountCreated) return Unit.Value;

            var acceptedContact = await _contactQueryRepository.GetContactById(contactInvitation.InviteeContactId);
            var invitingContact = await _contactQueryRepository.GetContactById(contactInvitation.InvitorContactId);
            var organisation = await _organisationQueryRepository.Get(contactInvitation.OrganisationId);

            var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate("EPAOLoginAccountCreated");

            await _mediator.Send(new SendEmailRequest(invitingContact.Email, emailTemplate, new
            {
                Contact = invitingContact.GivenNames,
                ContactName = acceptedContact.DisplayName,
                OrganisationName = organisation.EndPointAssessorName,
                ServiceTeam = "Apprenticeship assessment service team"
            }), cancellationToken);

            await _contactRepository.SetInvitationAccepted(contactInvitation);

            return Unit.Value;
        }
    }
}