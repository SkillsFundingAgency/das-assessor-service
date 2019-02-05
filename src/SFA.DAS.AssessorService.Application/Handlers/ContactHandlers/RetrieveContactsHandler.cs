using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class RetrieveContactsHandler : IRequestHandler<GetContactsRequest, List<ContactsWithRolesResponse>>
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        public RetrieveContactsHandler(IContactQueryRepository contactQueryRepository)
        {
            _contactQueryRepository = contactQueryRepository;
        }

        public async Task<List<ContactsWithRolesResponse>> Handle(GetContactsRequest request,
            CancellationToken cancellationToken)
        {
            var response = new List<ContactsWithRolesResponse>();
            var results = await _contactQueryRepository.GetAllContactsWithRoles(request.EndPointAssessorOrganisationId);
            if (results == null)
                return response;
            foreach (var result in results)
            {
                if (result.Key == null)
                    continue;
                var contactsWithRolesResponse = new ContactsWithRolesResponse
                {
                    EndPointAssessorOrganisationId= result.Key.EndPointAssessorOrganisationId,
                    PhoneNumber = result.Key.PhoneNumber,
                    DisplayName = result.Key.DisplayName,
                    Email = result.Key.Email,
                    Id = result.Key.Id,
                    Status = result.Key.Status == ContactStatus.Live?ContactStatus.Active: result.Key.Status
                };
                foreach (var role in result)
                {
                    contactsWithRolesResponse.Roles.Add(role.Role?.UserRole);
                }
                response.Add(contactsWithRolesResponse);
            }

            return response;
        }
    }
}
