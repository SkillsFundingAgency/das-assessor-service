using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetOptionsHandler : IRequestHandler<GetOptionsRequest, List<Option>>
    {
        private readonly IStandardRepository _repository;

        public GetOptionsHandler(IStandardRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<Option>> Handle(GetOptionsRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetOptions(request.StdCode);
        }
    }
}