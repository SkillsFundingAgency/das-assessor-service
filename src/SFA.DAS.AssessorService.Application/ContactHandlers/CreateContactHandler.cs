namespace SFA.DAS.AssessorService.Application.ContactHandlers
{
    using AssessorService.Api.Types.Models;
    using AutoMapper;
    using Domain;
    using MediatR;
    using Interfaces;
    using AssessorService.Domain.Enums;
    using System.Threading;
    using System.Threading.Tasks;

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
                

            var contactCreateDomainModel = Mapper.Map<ContactCreateDomainModel>(createContactRequest);                
            contactCreateDomainModel.OrganisationId = organisation.Id;

            if (!(await _organisationQueryRepository.CheckIfOrganisationHasContacts(createContactRequest.EndPointAssessorOrganisationId)))
            {
                contactCreateDomainModel.ContactStatus = ContactStatus.Live; // Not sure what to be done about this - to be confirmed?? 

                var contactQueryViewModel = await _contactRepository.CreateNewContact(contactCreateDomainModel);

                var organisationQueryDomainModel = await _organisationQueryRepository.Get(createContactRequest.EndPointAssessorOrganisationId);

                var organisationUpdateDomainModel =
                    Mapper.Map<OrganisationUpdateDomainModel>(organisationQueryDomainModel);

                organisationUpdateDomainModel.PrimaryContact = contactQueryViewModel.Username;
                organisationUpdateDomainModel.OrganisationStatus = OrganisationStatus.Live;

                await _organisationRepository.UpdateOrganisation(organisationUpdateDomainModel);

                return contactQueryViewModel;
            }
            else
            {
                var contactQueryViewModel = await _contactRepository.CreateNewContact(contactCreateDomainModel);
                return contactQueryViewModel;
            }           
        }
    }
}