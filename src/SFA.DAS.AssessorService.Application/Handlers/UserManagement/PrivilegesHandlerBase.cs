using SFA.DAS.AssessorService.Api.Types.Models.UserManagement;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.UserManagement
{
    public class PrivilegesHandlerBase
    {
        protected IContactQueryRepository ContactQueryRepository { get; }

        public PrivilegesHandlerBase(IContactQueryRepository contactQueryRepository)
        {
            ContactQueryRepository = contactQueryRepository;
        }

        protected async Task<List<ContactsPrivilege>> GetAddedPrivileges(SetContactPrivilegesRequest request, IList<ContactsPrivilege> currentPrivileges)
        {
            var allPrivileges = await ContactQueryRepository.GetAllPrivileges();

            return request.PrivilegeIds.Where(p => !currentPrivileges.Select(cp => cp.PrivilegeId).Contains(p))
                .Select(p => new ContactsPrivilege()
                {
                    PrivilegeId = p,
                    Privilege = allPrivileges.First(ap => ap.Id == p)
                }).ToList();
        }

        protected List<ContactsPrivilege> GetRemovedPrivileges(SetContactPrivilegesRequest request, IList<ContactsPrivilege> currentPrivileges)
        {
            return currentPrivileges.Where(cp => !request.PrivilegeIds.Contains(cp.PrivilegeId)).ToList();
        }
    }
}