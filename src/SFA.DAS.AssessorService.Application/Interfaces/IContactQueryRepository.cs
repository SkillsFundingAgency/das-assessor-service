using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Interfaces
{ 
    public interface IContactQueryRepository
    {
        Task<bool> CheckContactExists(string userName);

        Task<IEnumerable<ContactResponse>> GetContacts(string endPointAssessorOrganisationId);
        Task<ContactResponse> GetContact(string userName); 
    }
}