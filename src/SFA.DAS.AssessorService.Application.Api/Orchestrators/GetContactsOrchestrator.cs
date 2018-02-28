namespace SFA.DAS.AssessorService.Application.Api.Orchestrators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using Exceptions;
    using Interfaces;
    using Microsoft.Extensions.Logging;

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
            var contacts = (await _contactQueryRepository.GetContacts(endPointAssessorOrganisationId)).ToList();
            if (!contacts.Any())
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