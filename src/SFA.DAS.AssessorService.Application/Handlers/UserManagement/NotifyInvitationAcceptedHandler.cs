using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class NotifyInvitationAcceptedHandler : IRequestHandler<NotifyInvitationAcceptedRequest>
    {
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public NotifyInvitationAcceptedHandler(IEMailTemplateQueryRepository eMailTemplateQueryRepository, IContactQueryRepository contactQueryRepository, IMediator mediator, IOrganisationQueryRepository organisationQueryRepository)
        {
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _mediator = mediator;
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task Handle(NotifyInvitationAcceptedRequest message, CancellationToken cancellationToken)
        {
            const string invitationAcceptedTemplate = "EPAOLoginAccountCreated";

            var acceptedContact = await _contactQueryRepository.GetContactById(message.AcceptedContactId);
            var invitingContact = await _contactQueryRepository.GetContactById(message.InvitingContactId);
            var organisation = await _organisationQueryRepository.Get(invitingContact.EndPointAssessorOrganisationId);

            var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(invitationAcceptedTemplate);

            await _mediator.Send(new SendEmailRequest(invitingContact.Email, emailTemplate, new
            {
                Contact = invitingContact.DisplayName,
                ContactName = acceptedContact.DisplayName,
                OrganisationName = organisation.EndPointAssessorName,
                ServiceTeam = "Apprenticeship assessment service team"
            }), cancellationToken);
        }
    }
}
