using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class RetrieveContactsWithPrivilegesHandler : IRequestHandler<GetContactsWithPrivilegesRequest, List<ContactsWithPrivilegesResponse>>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        public RetrieveContactsWithPrivilegesHandler(IContactQueryRepository contactQueryRepository)
        {
            _contactQueryRepository = contactQueryRepository;
        }

        public async Task<List<ContactsWithPrivilegesResponse>> Handle(GetContactsWithPrivilegesRequest request,
            CancellationToken cancellationToken)
        {
            var response = new List<ContactsWithPrivilegesResponse>();
            var results = await _contactQueryRepository.GetAllContactsWithPrivileges(request.OrganisationId);
            if (results == null)
                return response;
            foreach (var result in results)
            {
                var contactsWithPrivilegesResponse = new ContactsWithPrivilegesResponse
                {
                    Contact = result
                };
                contactsWithPrivilegesResponse.Contact.Status = result.Status == ContactStatus.Live
                    ? ContactStatus.Active
                    : result.Status;
                foreach (var role in result.ContactsPrivileges)
                {
                    contactsWithPrivilegesResponse.Privileges.Add(new PrivilegeResponse
                    {
                        UserPrivilege = role.Privilege?.UserPrivilege,
                        Key = role.Privilege?.Key
                    });
                }

                contactsWithPrivilegesResponse.Contact.ContactsPrivileges = null;
                response.Add(contactsWithPrivilegesResponse);
            }

            return response;
        }
    }
}
