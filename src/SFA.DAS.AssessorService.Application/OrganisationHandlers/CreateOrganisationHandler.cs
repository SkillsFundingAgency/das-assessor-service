﻿namespace SFA.DAS.AssessorService.Application.OrganisationHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;    
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Domain.Enums;
    using SFA.DAS.AssessorService.ViewModel.Models;

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
            if (organisationCreateViewModel.PrimaryContactId.HasValue)
            {
                organisationCreateDomainModel.OrganisationStatus = OrganisationStatus.Live;
            }
            else
            {
                organisationCreateDomainModel.OrganisationStatus = OrganisationStatus.New;
            }

            var organisationQueryViewModel = await _organisationRepository.CreateNewOrganisation(organisationCreateDomainModel);
            return organisationQueryViewModel;            
        }
    }
}