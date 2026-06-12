using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.BatchLogs
{
    public class GetBatchLogHandler : BaseHandler, IRequestHandler<GetBatchLogRequest, BatchLogResponse>
    {
        private readonly IBatchLogQueryRepository _batchLogQueryRepository;

        public GetBatchLogHandler(IBatchLogQueryRepository batchLogQueryRepository, IMapper mapper)
            :base(mapper)
        {
            _batchLogQueryRepository = batchLogQueryRepository;
        }

        public async Task<BatchLogResponse> Handle(GetBatchLogRequest request, CancellationToken cancellationToken)
        {
            var batchLog = await _batchLogQueryRepository.Get(request.BatchNumber);
            return _mapper.Map<BatchLog, BatchLogResponse>(batchLog);
        }
    }
}
