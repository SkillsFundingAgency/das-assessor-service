using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetCalculatedStandardVersionForApprenticeshipHandler : IRequestHandler<GetCalculatedStandardVersionForApprenticeshipRequest, Standard>
    {
        private readonly IStandardService _standardService;
        private readonly ILearnerRepository _learnerRepository;

        public GetCalculatedStandardVersionForApprenticeshipHandler(IStandardService standardService, ILearnerRepository learnerRepository)
        {
            _standardService = standardService;
            _learnerRepository = learnerRepository;
        }
        public async Task<Standard> Handle(GetCalculatedStandardVersionForApprenticeshipRequest request, CancellationToken cancellationToken)
        {
            //request.StandardId will be IFateRef or LarsCode, this will get latest version of the standard
            var standard = await _standardService.GetStandardVersionById(request.StandardId);
            var learner = await _learnerRepository.Get(request.Uln, standard.LarsCode);

            if (learner == null)
            {
                // Can't calculate if no Learner/ILR record.
                return null;
            }

            var versions = await _standardService.GetStandardVersionsByLarsCode(standard.LarsCode);

            foreach (var version in versions.OrderBy(s => s.VersionMajor).ThenBy(t => t.VersionMinor))
            {
                if (learner.LearnStartDate <= version.VersionLatestStartDate || version.VersionLatestStartDate == null)
                {
                    return version;
                }
            }

            return standard;
        }
    }
}
