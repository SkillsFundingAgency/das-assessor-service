using System.Threading.Tasks;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.DomainModels;

namespace SFA.DAS.AssessorService.Application.Interfaces
{ 
    public interface IContactRepository
    {       
        Task<ContactResponse> CreateNewContact(CreateContactDomainModel newContact);
        Task Update(UpdateContactRequest updateContactRequest);
        Task Delete(string userName);
    }
}