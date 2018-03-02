namespace SFA.DAS.AssessorService.Application.OrganisationHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using AssessorService.Api.Types.Models;
    using AssessorService.Domain.Consts;
    using AutoMapper;
    using Domain;
    using Interfaces;
    using MediatR;

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
            organisationUpdateDomainModel.Status = string.IsNullOrEmpty(organisationUpdateViewModel.PrimaryContact) ? OrganisationStatus.New : OrganisationStatus.Live;

            var organisationQueryViewModel = await _organisationRepository.UpdateOrganisation(organisationUpdateDomainModel);
            return organisationQueryViewModel;
        }
    }
}