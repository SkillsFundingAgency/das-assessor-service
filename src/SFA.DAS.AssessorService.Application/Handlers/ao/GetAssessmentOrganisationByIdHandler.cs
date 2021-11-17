
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ao
{
    public class GetAssessmentOrganisationByIdHandler : IRequestHandler<GetAssessmentOrganisationByIdRequest, EpaOrganisation>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<GetAssessmentOrganisationByIdHandler> _logger;

        public GetAssessmentOrganisationByIdHandler(IRegisterQueryRepository registerQueryRepository, ILogger<GetAssessmentOrganisationByIdHandler> logger)
        {
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
        }

        public async Task<EpaOrganisation> Handle(AssessorService.Api.Types.Models.GetAssessmentOrganisationByIdRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($@"Handling AssessmentOrganisation Request for [{request.Id}]");
            var org = await _registerQueryRepository.GetEpaOrganisationById(request.Id);

            if (null != org)
            {
                AssessorService.Api.Types.Models.AO.OrganisationType orgType = null;
                var orgTypes = await _registerQueryRepository.GetOrganisationTypes();
                if (orgTypes != null)
                {
                    orgType = orgTypes.FirstOrDefault(x => x.Id == org.OrganisationTypeId);
                }
                org.FinancialReviewStatus = Helpers.FinancialReviewStatusHelper.IsFinancialExempt(org.OrganisationData?.FHADetails.FinancialExempt, org.OrganisationData?.FHADetails.FinancialDueDate, orgType) ? ApplyTypes.FinancialReviewStatus.Exempt : ApplyTypes.FinancialReviewStatus.Required;
            }

            return org ?? null;
        }  
    }
}
