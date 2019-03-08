using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetCollatedStandardHandler : IRequestHandler<GetCollatedStandardRequest, StandardCollation>
    {
        private readonly IStandardRepository _standardRepository;

        public GetCollatedStandardHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }
        public async Task<StandardCollation> Handle(GetCollatedStandardRequest request, CancellationToken cancellationToken)
        {
            StandardCollation result = null;

            if (request.StandardId.HasValue)
            {
                result = await _standardRepository.GetStandardCollationByStandardId(request.StandardId.Value);
            }
            else if (request.ReferenceNumber != null)
            {
                result = await _standardRepository.GetStandardCollationByReferenceNumber(request.ReferenceNumber);
            }

            return result;
        }
    }
}
