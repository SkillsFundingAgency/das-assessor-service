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

        public GetOrganisationStandardHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAssessmentOrganisationHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }


        public async Task<OrganisationStandard> Handle(GetOrganisationStandardRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling Get Organisation Request for [{request.OrganisationStandardId}]");
            var organisationStandard = await _registerQueryRepository.GetOrganisationStandardFromOrganisationStandardId(request.OrganisationStandardId);
            organisationStandard.Contact =
                await _registerQueryRepository.GetContactByContactId(organisationStandard.ContactId.GetValueOrDefault());
            var orgStandardDeliveryAreas =
                await _registerQueryRepository.GetDeliveryAreasByOrganisationStandardId(request.OrganisationStandardId);
            var allDeliveryAreas = await _registerQueryRepository.GetDeliveryAreas();
            foreach (var orgStandardDeliveryArea in orgStandardDeliveryAreas)
            {
                orgStandardDeliveryArea.DeliveryArea =
                    allDeliveryAreas.FirstOrDefault(x => x.Id == orgStandardDeliveryArea.DeliveryAreaId).Area;
            }

            organisationStandard.DeliveryAreas = orgStandardDeliveryAreas.ToList();


            return organisationStandard;      
        }
    }
}