using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class CreateContactHandler : IRequestHandler<CreateContactRequest, Contact>
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;
        private readonly IContactRepository _contactRepository;

        public CreateContactHandler(
            IOrganisationRepository organisationRepository,
            IOrganisationQueryRepository organisationQueryRepository,
            IContactRepository contactRepository)
        {
            _organisationRepository = organisationRepository;
            _contactRepository = contactRepository;
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<Contact> Handle(CreateContactRequest createContactRequest, CancellationToken cancellationToken)
        {
            var organisation = await _organisationQueryRepository.Get(createContactRequest.EndPointAssessorOrganisationId);
                
            var newContact = Mapper.Map<Contact>(createContactRequest);           
            newContact.OrganisationId = organisation.Id;
            newContact.Status = ContactStatus.InvitePending;

            if (!(await _organisationQueryRepository.CheckIfOrganisationHasContacts(createContactRequest.EndPointAssessorOrganisationId)))
            {
                var contactResponse = await _contactRepository.CreateNewContact(newContact);

                await SetOrganisationStatusToLiveAndSetPrimaryContact(createContactRequest, contactResponse);

                return contactResponse;
            }
            else
            {
                var contactResponse = await _contactRepository.CreateNewContact(newContact);
                return contactResponse;
            }           
        }

        private async Task SetOrganisationStatusToLiveAndSetPrimaryContact(CreateContactRequest createContactRequest, Contact contact)
        {
            var organisation =
                await _organisationQueryRepository.Get(createContactRequest.EndPointAssessorOrganisationId);
            
            organisation.PrimaryContact = contact.Username;
            organisation.Status = OrganisationStatus.Live;

            await _organisationRepository.UpdateOrganisation(organisation);
        }
    }
}