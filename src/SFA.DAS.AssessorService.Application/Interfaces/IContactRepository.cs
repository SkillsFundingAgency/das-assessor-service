namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IContactRepository
    {       
        Task<ContactQueryViewModel> CreateNewContact(ContactCreateDomainModel newContact);
        Task Update(ContactUpdateViewModel organisationUpdateViewModel);
        Task Delete(Guid id);
    }
}