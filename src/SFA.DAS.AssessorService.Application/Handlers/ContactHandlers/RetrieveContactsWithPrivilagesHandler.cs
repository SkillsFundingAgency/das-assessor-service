using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class RetrieveContactsWithPrivilagesHandler : IRequestHandler<GetContactsWithPrivilagesRequest, List<ContactsWithPrivilegesResponse>>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        public RetrieveContactsWithPrivilagesHandler(IContactQueryRepository contactQueryRepository)
        {
            _contactQueryRepository = contactQueryRepository;
        }

        public async Task<List<ContactsWithPrivilegesResponse>> Handle(GetContactsWithPrivilagesRequest request,
            CancellationToken cancellationToken)
        {
            var response = new List<ContactsWithPrivilegesResponse>();
            var results = await _contactQueryRepository.GetAllContactsWithPrivileges(request.EndPointAssessorOrganisationId);
            if (results == null)
                return response;
            foreach (var result in results)
            {
                if (result.Key == null)
                    continue;
                var contactsWithPrivilegesResponse = new ContactsWithPrivilegesResponse
                {
                    Contact = result.Key
                };
                contactsWithPrivilegesResponse.Contact.Status = result.Key.Status == ContactStatus.Live
                    ? ContactStatus.Active
                    : result.Key.Status;
                foreach (var role in result)
                {
                    contactsWithPrivilegesResponse.Privileges.Add(role.Privilege?.UserPrivilege);
                }
                response.Add(contactsWithPrivilegesResponse);
            }

            return response;
        }
    }
}
