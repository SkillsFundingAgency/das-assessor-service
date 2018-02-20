namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IContactRepository
    {
        Task<bool> CheckContactExists(int contactId);

        Task<IEnumerable<ContactQueryViewModel>> GetContacts(Guid id);
        Task<ContactQueryViewModel> GetContact(string userName, string emailAddress);

        Task<ContactQueryViewModel> CreateNewContact(ContactCreateDomainModel newContact);
    }
}