using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{ 
    public interface IContactRepository
    {       
        Task<Contact> CreateNewContact(Contact newContact);
        Task Update(UpdateContactRequest updateContactRequest);
        Task Delete(string userName);
        Task LinkOrganisation(string endPointAssessorOrganisationId, string userName);
    }
}