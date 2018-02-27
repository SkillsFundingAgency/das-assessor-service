namespace SFA.DAS.AssessorService.Application.Interfaces
{
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IContactRepository
    {       
        Task<Contact> CreateNewContact(ContactCreateDomainModel newContact);
        Task Update(UpdateContactRequest organisationUpdateViewModel);
        Task Delete(Guid id);
    }
}