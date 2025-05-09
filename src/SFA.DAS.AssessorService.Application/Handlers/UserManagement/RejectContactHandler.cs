using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Handlers.UserManagement
{
    public class RejectContactHandler : IRequestHandler<RejectContactRequest, Unit>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IMediator _mediator;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IContactRepository _contactRepository;

        public RejectContactHandler(IContactQueryRepository contactQueryRepository,
            IEMailTemplateQueryRepository eMailTemplateQueryRepository, IMediator mediator, IOrganisationQueryRepository organisationQueryRepository, IContactRepository contactRepository)
        {
            _contactQueryRepository = contactQueryRepository;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _mediator = mediator;
            _organisationQueryRepository = organisationQueryRepository;
            _contactRepository = contactRepository;
        }

        public async Task<Unit> Handle(RejectContactRequest message, CancellationToken cancellationToken)
        {
            const string epaoUserReject = "EPAOUserApproveReject";

            var contact = await _contactQueryRepository.GetContactById(message.ContactId);
            var organisation = await _organisationQueryRepository.Get(contact.OrganisationId.Value);

            await _contactRepository.UpdateStatus(message.ContactId, ContactStatus.New);
            await _contactRepository.UpdateOrganisationId(message.ContactId, null);
            
            var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(epaoUserReject);

            await _mediator.Send(new SendEmailRequest(contact.Email,
                emailTemplate, new
                {
                    Contact = $"{contact.DisplayName}",
                    ServiceName = "Apprenticeship assessment service",
                    Organisation = organisation.EndPointAssessorName,
                    ServiceTeam = "Apprenticeship assessment services team"
                }), cancellationToken);

            return Unit.Value;
        }
    }
}