using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{ 
    public interface IContactQueryRepository
    {
        Task<bool> CheckContactExists(string userName);

        Task<IEnumerable<Contact>> GetContacts(string endPointAssessorOrganisationId);
        Task<Contact> GetContact(string userName);
        Task<Contact> GetContactFromEmailAddress(string email);
        Task<IEnumerable<Contact>> GetAllContacts(string endPointAssessorOrganisationId);
        Task<Contact> GetContactById(Guid id);
    }
}