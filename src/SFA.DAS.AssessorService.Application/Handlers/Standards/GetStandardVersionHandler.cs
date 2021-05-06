using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetStandardVersionHandler : IRequestHandler<GetStandardVersionRequest, Standard>
    {
        private readonly IStandardService _standardService;

        public GetStandardVersionHandler(IStandardService standardService)
        {
            _standardService = standardService;
        }
        public async Task<Standard> Handle(GetStandardVersionRequest request, CancellationToken cancellationToken)
        {
            var result = await _standardService.GetStandardVersionById(request.StandardId, request.Version);

            return result;
        }
    }
}
