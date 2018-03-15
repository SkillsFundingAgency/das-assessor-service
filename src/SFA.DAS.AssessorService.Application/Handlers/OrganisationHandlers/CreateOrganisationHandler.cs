using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DomainModels;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class CreateOrganisationHandler : IRequestHandler<CreateOrganisationRequest, OrganisationResponse>
    {
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IContactRepository _contactRepository;    
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public CreateOrganisationHandler(IOrganisationRepository organisationRepository,                   
            IOrganisationQueryRepository organisationQueryRepository,
            IContactRepository contactRepository)
        {
            _organisationRepository = organisationRepository;
            _contactRepository = contactRepository;          
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<OrganisationResponse> Handle(CreateOrganisationRequest createOrganisationRequest, CancellationToken cancellationToken)
        {
            var organisation = await UpdateOrganisationIfExists(createOrganisationRequest)
                                             ?? await CreateNewOrganisation(createOrganisationRequest);

            return organisation;
        }

        private async Task<OrganisationResponse> CreateNewOrganisation(CreateOrganisationRequest createOrganisationRequest)
        {
            var createOrganisationDomainModel = Mapper.Map<CreateOrganisationDomainModel>(createOrganisationRequest);

            if (string.IsNullOrEmpty(createOrganisationRequest.PrimaryContact))
            {
                createOrganisationDomainModel.Status = OrganisationStatus.New;

                return await _organisationRepository.CreateNewOrganisation(createOrganisationDomainModel);
            }
            else
            {
                createOrganisationDomainModel.Status = OrganisationStatus.Live;
                createOrganisationDomainModel.PrimaryContact = createOrganisationRequest.PrimaryContact;

                var result = await _organisationRepository.CreateNewOrganisation(createOrganisationDomainModel);

                await _contactRepository.LinkOrganisation(createOrganisationDomainModel.EndPointAssessorOrganisationId,
                    createOrganisationDomainModel.PrimaryContact);

                return result;
            }          
        }

        private async Task<OrganisationResponse> UpdateOrganisationIfExists(CreateOrganisationRequest createOrganisationRequest)
        {
            var existingOrganisation =
                await _organisationQueryRepository.GetByUkPrn(createOrganisationRequest.EndPointAssessorUkprn);

            if (existingOrganisation != null
                && existingOrganisation.EndPointAssessorOrganisationId == createOrganisationRequest.EndPointAssessorOrganisationId
                && existingOrganisation.Status == OrganisationStatus.Deleted)
            {
                return await _organisationRepository.UpdateOrganisation(new UpdateOrganisationDomainModel
                {
                    EndPointAssessorName = createOrganisationRequest.EndPointAssessorName,
                    EndPointAssessorOrganisationId = existingOrganisation.EndPointAssessorOrganisationId,
                    PrimaryContact = existingOrganisation.PrimaryContact,
                    Status = string.IsNullOrEmpty(createOrganisationRequest.PrimaryContact)
                        ? OrganisationStatus.New
                        : OrganisationStatus.Live
                });
            }

            return null;
        }
    }
}