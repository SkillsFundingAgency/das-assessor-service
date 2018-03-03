﻿using System.Threading.Tasks;

using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.DomainModels;

namespace SFA.DAS.AssessorService.Application.Interfaces
{ 
    public interface IContactRepository
    {       
        Task<Contact> CreateNewContact(ContactCreateDomainModel newContact);
        Task Update(UpdateContactRequest organisationUpdateViewModel);
        Task Delete(string userName);
    }
}