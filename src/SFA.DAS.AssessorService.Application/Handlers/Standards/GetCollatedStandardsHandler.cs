using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;
using SFA.DAS.AssessorService.Web.Staff.Services;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetCollatedStandardsHandler : IRequestHandler<GetCollatedStandardsRequest, List<StandardCollation>>
    {
        private readonly IStandardRepository _standardRepository;

        public GetCollatedStandardsHandler(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }


        public async Task<List<StandardCollation>> Handle(GetCollatedStandardsRequest request, CancellationToken cancellationToken)
        {
            return await _standardRepository.GetStandardCollations();
        }
    }
}
