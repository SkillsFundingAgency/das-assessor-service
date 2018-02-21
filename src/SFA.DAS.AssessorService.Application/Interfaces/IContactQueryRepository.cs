namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IContactQueryRepository
    {
        Task<bool> CheckContactExists(int contactId);

        Task<IEnumerable<ContactQueryViewModel>> GetContacts(Guid id);
        Task<ContactQueryViewModel> GetContact(string userName, string emailAddress);    
    }
}