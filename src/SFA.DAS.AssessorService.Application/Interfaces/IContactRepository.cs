namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IContactRepository
    {
        Task<bool> CheckContactExists(int contactId);

        Task<IEnumerable<ContactQueryViewModel>> GetContacts(int ukprn);
        Task<ContactQueryViewModel> GetContact(int ukprn);
    }
}