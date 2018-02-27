namespace SFA.DAS.AssessorService.Application.OrganisationHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using AutoMapper;
    using Domain;
    using MediatR;
    using SFA.DAS.AssessorService.Api.Types;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Enums;

    public class UpdateOrganisationHandler : IRequestHandler<UpdateOrganisationRequest, Organisation>
    {
        private readonly IOrganisationRepository _organisationRepository;

        public UpdateOrganisationHandler(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<Organisation> Handle(UpdateOrganisationRequest organisationUpdateViewModel, CancellationToken cancellationToken)
        {
            var organisationUpdateDomainModel = Mapper.Map<OrganisationUpdateDomainModel>(organisationUpdateViewModel);
            if (organisationUpdateViewModel.PrimaryContactId.HasValue)
            {
                organisationUpdateDomainModel.OrganisationStatus = OrganisationStatus.Live;
            }
            else
            {
                organisationUpdateDomainModel.OrganisationStatus = OrganisationStatus.New;
            }

            var organisationQueryViewModel = await _organisationRepository.UpdateOrganisation(organisationUpdateDomainModel);
            return organisationQueryViewModel;
        }
    }
}