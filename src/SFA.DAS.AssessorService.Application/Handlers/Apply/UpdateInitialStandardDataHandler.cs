using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
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
            var application = await _applyRepository.GetApplication(request.Id);

            if (application != null)
            {
                request.ApplicationStatus = ApplicationStatus.InProgress;
                await _applyRepository.UpdateInitialStandardData(request);
                return true;
            }
            return false;
        }
    }
}
