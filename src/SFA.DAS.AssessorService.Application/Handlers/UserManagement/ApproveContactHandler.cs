using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class ApproveContactHandler : IRequestHandler<ApproveContactRequest>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IMediator _mediator;
        private readonly IWebConfiguration _config;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public ApproveContactHandler(IContactRepository contactRepository, IContactQueryRepository contactQueryRepository,
            IEMailTemplateQueryRepository eMailTemplateQueryRepository, IMediator mediator, IWebConfiguration config, IOrganisationQueryRepository organisationQueryRepository)
        {
            _contactRepository = contactRepository;
            _contactQueryRepository = contactQueryRepository;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _mediator = mediator;
            _config = config;
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<Unit> Handle(ApproveContactRequest message, CancellationToken cancellationToken)
        {
            const string epaoApproveConfirmTemplate = "EPAOUserApproveConfirm";

            var contact = await _contactQueryRepository.GetContactById(message.ContactId);
            var organisation = await _organisationQueryRepository.Get(contact.OrganisationId.Value);

            await _contactRepository.UpdateContactWithOrganisationData(new UpdateContactWithOrgAndStausRequest(message.ContactId.ToString(),
                organisation.Id.ToString(), organisation.EndPointAssessorOrganisationId, ContactStatus.Live));

            var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(epaoApproveConfirmTemplate);

            await _mediator.Send(new SendEmailRequest(contact.Email,
                emailTemplate, new
                {
                    Contact = $"{contact.DisplayName}",
                    ServiceName = "Apprenticeship assessment service",
                    Organisation = organisation.EndPointAssessorName,
                    LoginLink = _config.ServiceLink,
                    ServiceTeam = "Apprenticeship assessment services team"
                }), cancellationToken);
            return Unit.Value;
        }
    }
}