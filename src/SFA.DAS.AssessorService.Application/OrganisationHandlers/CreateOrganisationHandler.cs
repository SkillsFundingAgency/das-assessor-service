using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using AutoMapper;
using SFA.DAS.AssessorService.Application.Domain;
using SFA.DAS.AssessorService.Application.Interfaces;
using MediatR;

namespace SFA.DAS.AssessorService.Application.OrganisationHandlers
{
    using AssessorService.Domain.Consts;

    public class CreateOrganisationHandler : IRequestHandler<CreateOrganisationRequest, Organisation>
    {      
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public CreateOrganisationHandler(IOrganisationRepository organisationRepository, IOrganisationQueryRepository organisationQueryRepository)
        {
            _organisationRepository = organisationRepository;
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
            var organisationCreateDomainModel = Mapper.Map<OrganisationCreateDomainModel>(createOrganisationRequest);

            organisationCreateDomainModel.Status = string.IsNullOrEmpty(createOrganisationRequest.PrimaryContact)
                ? OrganisationStatus.New
                : OrganisationStatus.Live;

            return await _organisationRepository.CreateNewOrganisation(organisationCreateDomainModel);
        }

        private async Task<Organisation> UpdateOrganisationIfExists(CreateOrganisationRequest createOrganisationRequest)
        {
            var existingOrganisation =
                await _organisationQueryRepository.GetByUkPrn(createOrganisationRequest.EndPointAssessorUkprn);

            if (existingOrganisation != null
                && existingOrganisation.EndPointAssessorOrganisationId == createOrganisationRequest.EndPointAssessorOrganisationId
                && existingOrganisation.Status == OrganisationStatus.Deleted)
            {
                return await _organisationRepository.UpdateOrganisation(new OrganisationUpdateDomainModel
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