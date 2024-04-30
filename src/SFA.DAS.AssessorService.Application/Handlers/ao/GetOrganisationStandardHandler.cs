using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetOrganisationStandardHandler : IRequestHandler<GetOrganisationStandardRequest, OrganisationStandard>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAssessmentOrganisationHandler> _logger;
        private readonly IStandardService _standardService;

        public GetOrganisationStandardHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAssessmentOrganisationHandler> logger, IStandardService standardService)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
            _standardService = standardService;
        }


        public async Task<OrganisationStandard> Handle(GetOrganisationStandardRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling Get Organisation Request for [{request.OrganisationStandardId}]");
            var organisationStandard = await _registerQueryRepository.GetOrganisationStandardFromOrganisationStandardId(request.OrganisationStandardId);
            var organisation =
                await _registerQueryRepository.GetEpaOrganisationByOrganisationId(organisationStandard.OrganisationId);
            organisationStandard.OrganisationName = organisation?.Name;
            organisationStandard.OrganisationStatus = organisation?.Status;
            organisationStandard.Ukprn = organisation?.Ukprn;

            var standard = await _standardService.GetStandardVersionById(organisationStandard.IFateReferenceNumber);
            organisationStandard.StandardTitle = standard?.Title;
            organisationStandard.StandardEffectiveFrom = standard?.EffectiveFrom;
            organisationStandard.StandardEffectiveTo = standard?.EffectiveTo;
            organisationStandard.StandardLastDateForNewStarts = standard?.LastDateStarts;
            organisationStandard.IFateReferenceNumber = standard?.IfateReferenceNumber;
    
            var versions = await _standardService.GetEPAORegisteredStandardVersions(organisation?.OrganisationId, standard?.LarsCode);
            organisationStandard.Versions = versions.ToList();

            if (organisationStandard.ContactId != null)
                organisationStandard.Contact =  await _registerQueryRepository.GetContactByContactId(organisationStandard.ContactId.GetValueOrDefault());
            var orgStandardDeliveryAreas =
                await _registerQueryRepository.GetDeliveryAreasByOrganisationStandardId(request.OrganisationStandardId);
            var allDeliveryAreas = await _registerQueryRepository.GetDeliveryAreas();
            var organisationStandardDeliveryAreas = orgStandardDeliveryAreas as OrganisationStandardDeliveryArea[] ?? orgStandardDeliveryAreas.ToArray();
            if (organisationStandardDeliveryAreas.Any())
            {
                foreach (var orgStandardDeliveryArea in organisationStandardDeliveryAreas)
                {
                        orgStandardDeliveryArea.DeliveryArea =
                        allDeliveryAreas.FirstOrDefault(x => x.Id == orgStandardDeliveryArea.DeliveryAreaId)?.Area;
                }
            }

            organisationStandard.DeliveryAreasDetails = organisationStandardDeliveryAreas.ToList();
            organisationStandard.DeliveryAreas = organisationStandardDeliveryAreas.Select(orgStandardDeliveryArea => orgStandardDeliveryArea.DeliveryAreaId).ToList();

            return organisationStandard;      
        }
    }
}