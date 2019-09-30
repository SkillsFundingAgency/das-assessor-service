using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Review
{
    public class ReturnApplicationHandler : IRequestHandler<ReturnApplicationRequest>
    {
        public ReturnApplicationHandler()
        {
        }

        public async Task<Unit> Handle(ReturnApplicationRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Migrate code over");
        }
    }
}
