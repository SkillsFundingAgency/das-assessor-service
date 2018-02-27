namespace SFA.DAS.AssessorService.Application.OrganisationHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using AssessorService.Domain.Enums;
    using AutoMapper;
    using Domain;
    using Interfaces;
    using MediatR;

    public class CreateOrganisationHandler : IRequestHandler<CreateOrganisationRequest, Organisation>
    {      
        private readonly IOrganisationRepository _organisationRepository;

        public CreateOrganisationHandler(IOrganisationRepository organisationRepository)
        {            
            _organisationRepository = organisationRepository;
        }

        public async Task<Organisation> Handle(CreateOrganisationRequest organisationCreateViewModel, CancellationToken cancellationToken)
        {
            var organisationCreateDomainModel = Mapper.Map<OrganisationCreateDomainModel>(organisationCreateViewModel);
            organisationCreateDomainModel.OrganisationStatus = organisationCreateViewModel.PrimaryContactId.HasValue ? OrganisationStatus.Live : OrganisationStatus.New;

            var organisationQueryViewModel = await _organisationRepository.CreateNewOrganisation(organisationCreateDomainModel);
            return organisationQueryViewModel;            
        }
    }
}