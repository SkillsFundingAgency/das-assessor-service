using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class ApproveContactHandler : IRequestHandler<ApproveContactRequest, Unit>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IApiConfiguration _config;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IMediator _mediator;

        public ApproveContactHandler(IContactRepository contactRepository,
            IContactQueryRepository contactQueryRepository,
            IApiConfiguration config,
            IOrganisationQueryRepository organisationQueryRepository,
            IMediator mediator)
        {
            _contactRepository = contactRepository;
            _contactQueryRepository = contactQueryRepository;
            _config = config;
            _organisationQueryRepository = organisationQueryRepository;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ApproveContactRequest message, CancellationToken cancellationToken)
        {
            var contact = await _contactQueryRepository.GetContactById(message.ContactId);
            var organisation = await _organisationQueryRepository.Get(contact.OrganisationId.Value);

            await _contactRepository.UpdateContactWithOrganisationData(new UpdateContactWithOrgAndStausRequest(message.ContactId.ToString(),
                organisation.Id.ToString(), organisation.EndPointAssessorOrganisationId, ContactStatus.Live));
            
            
            // send approve confirmation email to the user with service link.
            await _mediator.Send(new SendEmailRequest(contact.Email, new EmailTemplateSummary
            {
                TemplateId = _config.EmailTemplatesConfig.UserApproveConfirm,
                TemplateName = nameof(_config.EmailTemplatesConfig.UserApproveConfirm)
            }, new
            {
                name = $"{contact.DisplayName}",
                organisation = organisation.EndPointAssessorName,
                link = _config.ServiceLink
            }), cancellationToken);

            return Unit.Value;
        }
    }
}