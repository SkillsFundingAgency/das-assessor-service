﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.StandardCollationApiClient.Types;

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
            return await _standardRepository.GetStandardCollationByStandardId(request.StandardId);
        }
    }
}
