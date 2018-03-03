using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DomainModels;

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
                
            var contactCreateDomainModel = Mapper.Map<ContactCreateDomainModel>(createContactRequest);           
            contactCreateDomainModel.OrganisationId = organisation.Id;

            if (!(await _organisationQueryRepository.CheckIfOrganisationHasContacts(createContactRequest.EndPointAssessorOrganisationId)))
            {
                var contactQueryViewModel = await _contactRepository.CreateNewContact(contactCreateDomainModel);

                await SetOrganisationStatusToLiveAndSetPrimaryContact(createContactRequest, contactQueryViewModel);

                return contactQueryViewModel;
            }
            else
            {
                var contactQueryViewModel = await _contactRepository.CreateNewContact(contactCreateDomainModel);
                return contactQueryViewModel;
            }           
        }

        private async Task SetOrganisationStatusToLiveAndSetPrimaryContact(CreateContactRequest createContactRequest, Contact contactQueryViewModel)
        {
            var organisationQueryDomainModel =
                await _organisationQueryRepository.Get(createContactRequest.EndPointAssessorOrganisationId);
            var organisationUpdateDomainModel =
                Mapper.Map<OrganisationUpdateDomainModel>(organisationQueryDomainModel);
            organisationUpdateDomainModel.PrimaryContact = contactQueryViewModel.Username;
            organisationUpdateDomainModel.Status = OrganisationStatus.Live;

            await _organisationRepository.UpdateOrganisation(organisationUpdateDomainModel);
        }
    }
}