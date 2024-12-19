using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class NotifyUserManagementUsersHandler : IRequestHandler<NotifyUserManagementUsersRequest, Unit>
    {
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IMediator _mediator;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public NotifyUserManagementUsersHandler(IEMailTemplateQueryRepository eMailTemplateQueryRepository, IContactQueryRepository contactQueryRepository, IMediator mediator, IOrganisationQueryRepository organisationQueryRepository)
        {
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _contactQueryRepository = contactQueryRepository;
            _mediator = mediator;
            _organisationQueryRepository = organisationQueryRepository;
        }
        
        public async Task<Unit> Handle(NotifyUserManagementUsersRequest message, CancellationToken cancellationToken)
        {
            const string epaoUserApproveRequestTemplate = "EPAOUserApproveRequest";

            var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(epaoUserApproveRequestTemplate);

            var organisation = await _organisationQueryRepository.Get(message.EndPointAssessorOrganisationId);
            
            var contactsWithUserManagementPrivilege = (await _contactQueryRepository.GetAllContactsIncludePrivileges(organisation.EndPointAssessorOrganisationId))
                .Where(c => c.ContactsPrivileges.Any(cp => cp.Privilege.Key == Privileges.ManageUsers && 
                    cp.Contact.Status == ContactStatus.Live)).ToList();
            
            foreach (var contact in contactsWithUserManagementPrivilege)
            {
                await _mediator.Send(new SendEmailRequest(contact.Email, emailTemplate, new
                {
                    Contact = contact.DisplayName,
                    username = message.DisplayName,
                    ServiceName = "Apprenticeship assessment service",
                    LoginLink = message.ServiceLink,
                    ServiceTeam = "Apprenticeship assessment service team",
                    Organisation = organisation.EndPointAssessorName
                }), cancellationToken);
            }
            return Unit.Value;
        }
    }
}