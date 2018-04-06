using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class CreateOrganisationHandler : IRequestHandler<CreateOrganisationRequest, Organisation>
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

        public async Task<Organisation> Handle(CreateOrganisationRequest createOrganisationRequest, CancellationToken cancellationToken)
        {
            var organisation = await UpdateOrganisationIfExists(createOrganisationRequest)
                                             ?? await CreateNewOrganisation(createOrganisationRequest);

            return organisation;
        }

        private async Task<Organisation> CreateNewOrganisation(CreateOrganisationRequest createOrganisationRequest)
        {
            var organisation = Mapper.Map<Organisation>(createOrganisationRequest);
            
            organisation.Status = OrganisationStatus.New;

            return await _organisationRepository.CreateNewOrganisation(organisation);
        }

        private async Task<Organisation> UpdateOrganisationIfExists(CreateOrganisationRequest createOrganisationRequest)
        {
            if (!createOrganisationRequest.EndPointAssessorUkprn.HasValue)
            {
                return null;
            }

            var existingOrganisation =
                await _organisationQueryRepository.GetByUkPrn(createOrganisationRequest.EndPointAssessorUkprn.Value);

            if (existingOrganisation != null
                && existingOrganisation.EndPointAssessorOrganisationId == createOrganisationRequest.EndPointAssessorOrganisationId
                && existingOrganisation.Status == OrganisationStatus.Deleted)
            {
                return await _organisationRepository.UpdateOrganisation(new Organisation
                {
                    EndPointAssessorName = createOrganisationRequest.EndPointAssessorName,
                    EndPointAssessorOrganisationId = existingOrganisation.EndPointAssessorOrganisationId,
                    PrimaryContact = existingOrganisation.PrimaryContact,
                    Status = OrganisationStatus.New
                     
                });
            }

            return null;
        }
    }
}