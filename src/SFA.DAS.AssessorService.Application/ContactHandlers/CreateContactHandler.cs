namespace SFA.DAS.AssessorService.Application.ContactHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;  
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Enums;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class CreateContactHandler : IRequestHandler<ContactCreateViewModel, ContactQueryViewModel>
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IContactRepository _contactRepository;

        public CreateContactHandler(
            IOrganisationRepository organisationRepository,
            IContactRepository contactRepository)
        {
            _organisationRepository = organisationRepository;
            _contactRepository = contactRepository;
        }

        public async Task<ContactQueryViewModel> Handle(ContactCreateViewModel contactCreateViewModel, CancellationToken cancellationToken)
        {
            var contactCreateDomainModel = Mapper.Map<ContactCreateDomainModel>(contactCreateViewModel);
            contactCreateDomainModel.ContactStatus = ContactStatus.Live; // Not sure what to be done about this - to be confirmed??      

            if (!(await _organisationRepository.CheckIfOrganisationHasContacts(contactCreateViewModel.OrganisationId)))
            {
                var contactQueryViewModel = await _contactRepository.CreateNewContact(contactCreateDomainModel);

                var organisationDomainModel = await _organisationRepository.Get(contactCreateViewModel.OrganisationId);
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