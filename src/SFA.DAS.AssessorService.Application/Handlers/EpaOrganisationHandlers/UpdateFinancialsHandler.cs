using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateFinancialsHandler : IRequestHandler<UpdateFinancialsRequest>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IRegisterQueryRepository _registerQueryRepository;

        public UpdateFinancialsHandler(IRegisterRepository registerRepository, IRegisterQueryRepository registerQueryRepository)
        {
            _registerRepository = registerRepository;
            _registerQueryRepository = registerQueryRepository;
        }
        
        public async Task Handle(UpdateFinancialsRequest message, CancellationToken cancellationToken)
        {
            var epaOrg = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(message.EpaOrgId);
 
            epaOrg.OrganisationData.FHADetails = new FHADetails()
            {
                FinancialDueDate = message.FinancialDueDate,
                FinancialExempt = message.FinancialExempt
            };

            await _registerRepository.UpdateEpaOrganisation(epaOrg);
        }
    }
}