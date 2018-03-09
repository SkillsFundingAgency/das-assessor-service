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
    public class UpdateOrganisationHandler : IRequestHandler<UpdateOrganisationRequest, OrganisationResponse>
    {
        private readonly IOrganisationRepository _organisationRepository;

        public UpdateOrganisationHandler(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<OrganisationResponse> Handle(UpdateOrganisationRequest updateOrganisationRequest, CancellationToken cancellationToken)
        {
            var organisationUpdateDomainModel = Mapper.Map<UpdateOrganisationDomainModel>(updateOrganisationRequest);
            organisationUpdateDomainModel.Status = string.IsNullOrEmpty(updateOrganisationRequest.PrimaryContact) ? OrganisationStatus.New : OrganisationStatus.Live;

            var organisationRespone = await _organisationRepository.UpdateOrganisation(organisationUpdateDomainModel);
            return organisationRespone;
        }
    }
}