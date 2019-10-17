using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class GetAllContactsIncludePrivilegesHandler : IRequestHandler<GetAllContactsIncludePrivilegesRequest, List<ContactIncludePrivilegesResponse>>
    {
        private readonly IContactQueryRepository _contactQueryRepository;

        public GetAllContactsIncludePrivilegesHandler(IContactQueryRepository contactQueryRepository)
        {
            _contactQueryRepository = contactQueryRepository;
        }

        public async Task<List<ContactIncludePrivilegesResponse>> Handle(GetAllContactsIncludePrivilegesRequest request,
            CancellationToken cancellationToken)
        {
            var response = new List<ContactIncludePrivilegesResponse>();
            var results = await _contactQueryRepository.GetAllContactsIncludePrivileges(request.EndPointAssessorOrganisationId, request.WithUser);

            if (results == null)
                return response;

            foreach (var result in results)
            {
                var contactsWithPrivilegesResponse = new ContactIncludePrivilegesResponse
                {
                    Contact = Mapper.Map<ContactResponse>(result)
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

                response.Add(contactsWithPrivilegesResponse);
            }

            return response;
        }
    }
}
