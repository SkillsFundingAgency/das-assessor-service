namespace SFA.DAS.AssessorService.Application.OrganisationHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;    
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Enums;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class UpdateOrganisationHandler : IRequestHandler<OrganisationUpdateViewModel, OrganisationQueryViewModel>
    {
        private readonly IOrganisationRepository _organisationRepository;

        public UpdateOrganisationHandler(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<OrganisationQueryViewModel> Handle(OrganisationUpdateViewModel organisationUpdateViewModel, CancellationToken cancellationToken)
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