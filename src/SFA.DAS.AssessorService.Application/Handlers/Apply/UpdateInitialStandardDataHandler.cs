using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class UpdateInitialStandardDataHandler : IRequestHandler<UpdateInitialStandardDataRequest, bool>
    {
        private readonly IApplyRepository _applyRepository;

        public UpdateInitialStandardDataHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }

        public async Task<bool> Handle(UpdateInitialStandardDataRequest request, CancellationToken cancellationToken)
        {
            return await _applyRepository.UpdateStandardData(request.Id, request.StandardCode, request.ReferenceNumber, request.StandardName);
        }
    }
}
