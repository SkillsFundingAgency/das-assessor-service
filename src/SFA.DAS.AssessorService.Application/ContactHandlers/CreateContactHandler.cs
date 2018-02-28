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

        public async Task<Contact> Handle(CreateContactRequest contactCreateViewModel, CancellationToken cancellationToken)
        {
            var contactCreateDomainModel = Mapper.Map<ContactCreateDomainModel>(contactCreateViewModel);
            contactCreateDomainModel.ContactStatus = ContactStatus.Live; // Not sure what to be done about this - to be confirmed??      

            if (!(await _organisationQueryRepository.CheckIfOrganisationHasContacts(contactCreateViewModel.OrganisationId)))
            {
                var contactQueryViewModel = await _contactRepository.CreateNewContact(contactCreateDomainModel);

                var organisationDomainModel = await _organisationQueryRepository.Get(contactCreateViewModel.OrganisationId);
                organisationDomainModel.PrimaryContactId = contactQueryViewModel.Id;
                organisationDomainModel.OrganisationStatus = OrganisationStatus.Live;

                await _organisationRepository.UpdateOrganisation(organisationDomainModel);

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