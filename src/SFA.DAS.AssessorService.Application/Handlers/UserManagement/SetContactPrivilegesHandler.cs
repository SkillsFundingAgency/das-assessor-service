using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Handlers.UserManagement;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class SetContactPrivilegesHandler : PrivilegesHandlerBase, IRequestHandler<SetContactPrivilegesRequest, SetContactPrivilegesResponse>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMediator _mediator;

        public SetContactPrivilegesHandler(IContactRepository contactRepository, IContactQueryRepository contactQueryRepository, IMediator mediator) : base(contactQueryRepository)
        {
            _contactRepository = contactRepository;
            _mediator = mediator;
        }

        public async Task<SetContactPrivilegesResponse> Handle(SetContactPrivilegesRequest request, CancellationToken cancellationToken)
        {
            var currentPrivileges = (await ContactQueryRepository.GetPrivilegesFor(request.ContactId));
            var privilegesBeingRemoved = GetRemovedPrivileges(request, currentPrivileges);
            var privilegesBeingAdded = await GetAddedPrivileges(request, currentPrivileges);

            var privilegesBeingRemovedThatMustBelongToSomeone = privilegesBeingRemoved.Where(p => p.Privilege.MustBeAtLeastOneUserAssigned).ToList();

            foreach (var currentPrivilege in privilegesBeingRemovedThatMustBelongToSomeone)
            {
                if (await _contactRepository.IsOnlyContactWithPrivilege(request.ContactId, currentPrivilege.PrivilegeId))
                {
                    return new SetContactPrivilegesResponse()
                    {
                        Success = false,
                        ErrorMessage = $"Before you remove '{currentPrivilege.Privilege.UserPrivilege}' you must assign '{currentPrivilege.Privilege.UserPrivilege}' to another user."
                    };
                }
            }

            await UpdatePrivileges(request);

            if (!request.IsNewContact)
            {
                await SendEmail(privilegesBeingAdded, privilegesBeingRemoved, request);

                await LogPrivilegeChanges(privilegesBeingAdded, privilegesBeingRemoved, request);
            }

            return new SetContactPrivilegesResponse()
            {
                Success = true,
                HasRemovedOwnUserManagement = request.ContactId.Equals(request.AmendingContactId) && privilegesBeingRemovedThatMustBelongToSomeone.Any(p => p.Privilege.Key == Privileges.ManageUsers)
            };
        }

        private async Task LogPrivilegeChanges(List<ContactsPrivilege> privilegesBeingAdded, List<ContactsPrivilege> privilegesBeingRemoved, SetContactPrivilegesRequest request)
        {
            await _contactRepository.CreateContactLog(
                request.AmendingContactId,
                request.ContactId,
                request.AmendingContactId.Equals(Guid.Empty)
                    ? ContactLogType.PrivilegesAmendedByStaff
                    : ContactLogType.PrivilegesAmended,
                new
                {
                    PrivilegesAdded = privilegesBeingAdded.Select(p => p.Privilege.UserPrivilege),
                    PrivilegesRemoved = privilegesBeingRemoved.Select(p => p.Privilege.UserPrivilege)
                });
        }

        private async Task SendEmail(List<ContactsPrivilege> privilegesBeingAdded, List<ContactsPrivilege> privilegesBeingRemoved, SetContactPrivilegesRequest request)
        {
            if (privilegesBeingAdded.Any() || privilegesBeingRemoved.Any())
            {
                var removedText = GetRemovedPrivilegesEmailToken(privilegesBeingRemoved);
                var addedText = GetAddedPrivilegesEmailToken(privilegesBeingAdded);

                var emailTemplate = await _mediator.Send(new GetEmailTemplateRequest
                { TemplateName = "EPAOPermissionsAmended" });

                var amendingContact =
                    request.AmendingContactId.Equals(Guid.Empty)
                        ? null
                        : await ContactQueryRepository.GetContactById(request.AmendingContactId);

                var amendingContactDisplayName = 
                    amendingContact == null
                    ? "ESFA Staff"
                    : amendingContact.DisplayName;


                var contactBeingAmended = await ContactQueryRepository.GetContactById(request.ContactId);

                await _mediator.Send(new SendEmailRequest(contactBeingAmended.Email, emailTemplate, new
                {
                    ServiceName = "Apprenticeship assessment service",
                    Contact = contactBeingAmended.DisplayName,
                    Editor = amendingContactDisplayName,
                    ServiceTeam = "Apprenticeship assessment service team",
                    PermissionsAdded = addedText,
                    PermissionsRemoved = removedText
                }));
            }
        }

        private string GetAddedPrivilegesEmailToken(List<ContactsPrivilege> privilegesBeingAdded)
        {
            var addedText = "";
            if (privilegesBeingAdded.Any())
            {
                addedText = @"The following permissions have been added:" + Environment.NewLine;
                foreach (var added in privilegesBeingAdded)
                {
                    addedText += " • " + added.Privilege.UserPrivilege + Environment.NewLine;
                }
            }

            return addedText;
        }

        private string GetRemovedPrivilegesEmailToken(List<ContactsPrivilege> privilegesBeingRemoved)
        {
            var removedText = "";
            if (privilegesBeingRemoved.Any())
            {
                removedText = @"The following permissions have been removed:" + Environment.NewLine;
                foreach (var added in privilegesBeingRemoved)
                {
                    removedText += " • " + added.Privilege.UserPrivilege + Environment.NewLine;
                }
            }

            return removedText;
        }

        private async Task UpdatePrivileges(SetContactPrivilegesRequest request)
        {
            await _contactRepository.RemoveAllPrivileges(request.ContactId);

            foreach (var privilegeId in request.PrivilegeIds)
            {
                await _contactRepository.AddPrivilege(request.ContactId, privilegeId);
            }
        }


    }
}