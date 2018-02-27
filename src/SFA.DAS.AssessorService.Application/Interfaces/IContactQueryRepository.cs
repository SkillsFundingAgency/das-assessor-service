namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IContactQueryRepository
    {
        Task<bool> CheckContactExists(int contactId);
        Task<bool> CheckContactExists(Guid value);
        Task<bool> CheckContactExists(string userName, string emailAddress);

        Task<IEnumerable<Contact>> GetContacts(string endPointAssessorOrganisationId);
        Task<Contact> GetContact(string userName);
      
    }
}