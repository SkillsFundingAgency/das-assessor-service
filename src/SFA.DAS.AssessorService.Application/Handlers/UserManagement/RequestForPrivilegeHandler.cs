using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class RequestForPrivilegeHandler : IRequestHandler<RequestForPrivilegeRequest>
    {
        private readonly IMediator _mediator;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IWebConfiguration _config;
        private readonly IContactRepository _contactRepository;

        public RequestForPrivilegeHandler(IMediator mediator, IContactQueryRepository contactQueryRepository, 
            IOrganisationQueryRepository organisationQueryRepository, IWebConfiguration config, IContactRepository contactRepository)
        {
            _mediator = mediator;
            _contactQueryRepository = contactQueryRepository;
            _organisationQueryRepository = organisationQueryRepository;
            _config = config;
            _contactRepository = contactRepository;
        }
        
        public async Task Handle(RequestForPrivilegeRequest message, CancellationToken cancellationToken)
        {
            var privilege = (await _contactQueryRepository.GetAllPrivileges()).Single(p => p.Id == message.PrivilegeId);
            
            var requestingContact = await _contactQueryRepository.GetContactById(message.ContactId);
            var organisation = await _organisationQueryRepository.GetOrganisationByContactId(message.ContactId);

            var contactsWithUserManagementPrivilege = (await _contactQueryRepository.GetAllContactsWithPrivileges(organisation.Id))
                .Where(c => c.ContactsPrivileges.Any(cp => cp.Privilege.Key == Privileges.ManageUsers && 
                    cp.Contact.Status == ContactStatus.Live)).ToList();

            if (RequestingUserHasUserManagementPrivilege(contactsWithUserManagementPrivilege, requestingContact))
            {
                await _contactRepository.AddPrivilege(requestingContact.Id, message.PrivilegeId);
            }
            else
            {
                var emailTemplate = await _mediator.Send(new GetEmailTemplateRequest{TemplateName= "EPAOPermissionsRequested" }, cancellationToken);
            
                contactsWithUserManagementPrivilege.ForEach(async contact =>
                {
                    await _mediator.Send(new SendEmailRequest(contact.Email, emailTemplate, new
                    {
                        ServiceName = "Apprenticeship assessment service",
                        Contact = contact.DisplayName,
                        Username = requestingContact.DisplayName,
                        Permission = privilege.UserPrivilege,
                        ServiceTeam = "Apprenticeship assessment service team",
                        LoginLink = _config.ServiceLink
                    }));
                });   
            }
        }

        private static bool RequestingUserHasUserManagementPrivilege(List<Contact> contactsWithUserManagementPrivilege, Contact requestingContact)
        {
            return contactsWithUserManagementPrivilege.Any(c => c.Id == requestingContact.Id);
        }
    }
}