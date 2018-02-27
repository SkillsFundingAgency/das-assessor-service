namespace SFA.DAS.AssessorService.Application.Api.Orchesrators
{
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Application.Exceptions;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class GetContactsOrchestrator
    {
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly ILogger<GetContactsOrchestrator> _logger;

        public GetContactsOrchestrator(IContactQueryRepository contactQueryRepository,
            ILogger<GetContactsOrchestrator> logger)
        {
            _contactQueryRepository = contactQueryRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Contact>> SearchContactsForAnOrganisation(string endPointAssessorOrganisationId)
        {
            var contacts = await _contactQueryRepository.GetContacts(endPointAssessorOrganisationId);
            if (contacts.Count() == 0)
                throw new ResourceNotFoundException();
            return contacts;
        }

        public async Task<Contact> SearchContactByUserName(string userName)
        {
            var contact = await _contactQueryRepository.GetContact(userName);
            if (contact == null)
                throw new ResourceNotFoundException();
            return contact;
        }
    }
}


