namespace SFA.DAS.AssessorService.Application.CreateOrganisation
{
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using SFA.DAS.AssessorService.Application.Api.Consts;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.ViewModel.Models;

    public class CreateOrganisationHandler : IRequestHandler<OrganisationCreateViewModel, OrganisationQueryViewModel>
    {      
        private readonly IOrganisationRepository _organisationRepository;

        public CreateOrganisationHandler(IOrganisationRepository organisationRepository)
        {            
            _organisationRepository = organisationRepository;
        }

        public async Task<OrganisationQueryViewModel> Handle(OrganisationCreateViewModel organisationCreateViewModel, CancellationToken cancellationToken)
        {
            var organisationCreateDomainModel = Mapper.Map<OrganisationCreateDomainModel>(organisationCreateViewModel);
            if (organisationCreateViewModel.PrimaryContactId.HasValue)
            {
                organisationCreateDomainModel.Status = OrganisationStatus.Live;
            }
            else
            {
                organisationCreateDomainModel.Status = OrganisationStatus.New;
            }

            var organisationQueryViewModel = await _organisationRepository.CreateNewOrganisation(organisationCreateDomainModel);
            return organisationQueryViewModel;            
        }
    }
}