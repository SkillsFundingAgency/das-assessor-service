using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.Mapping.Structs;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetCalculatedStandardVersionForApprenticeshipHandler : IRequestHandler<GetCalculatedStandardVersionForApprenticeshipRequest, Standard>
    {
        private readonly IStandardService _standardService;
        private readonly IIlrRepository _ilrRepository;

        public GetCalculatedStandardVersionForApprenticeshipHandler(IStandardService standardService, IIlrRepository ilrRepository)
        {
            _standardService = standardService;
            _ilrRepository = ilrRepository;
        }
        public async Task<Standard> Handle(GetCalculatedStandardVersionForApprenticeshipRequest request, CancellationToken cancellationToken)
        {
            var standardId = new StandardId(request.StandardId);
            int larsCode;
            if (standardId.IdType != StandardId.StandardIdType.LarsCode)
            {
                var result = await _standardService.GetStandardVersionById(request.StandardId);
                larsCode = result.LarsCode;
            }
            else
            {
                larsCode = standardId.LarsCode;
            }

            var ilr = await _ilrRepository.Get(request.Uln, larsCode);
            var versions = await _standardService.GetStandardVersionsByLarsCode(larsCode);

            foreach (var version in versions.OrderBy(s => s.VersionEarliestStartDate))
            {
                if (ilr.LearnStartDate > version.VersionEarliestStartDate && (ilr.LearnStartDate < version.VersionLatestStartDate || version.VersionLatestStartDate == null))
                {
                    return version;
                }
            }

            throw new InvalidOperationException($"Unable to find a version for Standard {request.StandardId} that accepts earliest start date for learner with date {ilr.LearnStartDate}");
        }
    }
}
