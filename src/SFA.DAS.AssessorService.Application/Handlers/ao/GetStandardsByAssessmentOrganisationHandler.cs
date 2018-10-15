using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Web.Staff.Services;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetStandardsByAssessmentOrganisationHandler: IRequestHandler<GetStandardsByOrganisationRequest, List<OrganisationStandardSummary>>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetStandardsByAssessmentOrganisationHandler> _logger;
        private readonly IStandardService _standardService;

        public GetStandardsByAssessmentOrganisationHandler(IRegisterQueryRepository registerQueryRepository, IStandardService standardService, ILogger<GetStandardsByAssessmentOrganisationHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _standardService = standardService;
            _logger = logger;
        }

        public async Task<List<OrganisationStandardSummary>> Handle(GetStandardsByOrganisationRequest request, CancellationToken cancellationToken)
        {
            var organisationId = request.OrganisationId;
            _logger.LogInformation($@"Handling OrganisationStandards Request for OrganisationId [{organisationId}]");
            var orgStandards = await _registerQueryRepository.GetOrganisationStandardByOrganisationId(organisationId);
            foreach (var orgStandard in orgStandards)
            {
                var deliveryAreas = await _registerQueryRepository.GetDeliveryAreaIdsByOrganisationStandardId(orgStandard.Id);
                orgStandard.DeliveryAreas = deliveryAreas.ToList();
            }
            
            var allStandards = _standardService.GetAllStandardSummaries().Result;

            foreach (var organisationStandard in orgStandards)
            {
                var std = allStandards.First(x => x.Id == organisationStandard.StandardCode.ToString());
                organisationStandard.StandardSummary = std;
            }
            return orgStandards.ToList();
        }
    } 
}
