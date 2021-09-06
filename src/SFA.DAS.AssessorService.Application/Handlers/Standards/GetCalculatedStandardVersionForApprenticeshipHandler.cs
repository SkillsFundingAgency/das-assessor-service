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
        private readonly IIlrRepository _ilrRepository;

        public GetCalculatedStandardVersionForApprenticeshipHandler(IStandardService standardService, IIlrRepository ilrRepository)
        {
            _standardService = standardService;
            _ilrRepository = ilrRepository;
        }
        public async Task<Standard> Handle(GetCalculatedStandardVersionForApprenticeshipRequest request, CancellationToken cancellationToken)
        {
            //request.StandardId will be IFateRef or LarsCode, this will get latest version of the standard
            var standard = await _standardService.GetStandardVersionById(request.StandardId);
            var ilr = await _ilrRepository.Get(request.Uln, standard.LarsCode);
            
            if(ilr == null)
            {
                // Can't calculate if no ILR record.
                return null;
            }
            
            var versions = await _standardService.GetStandardVersionsByLarsCode(standard.LarsCode);

            foreach (var version in versions.OrderBy(s => s.VersionMajor).ThenBy(t => t.VersionMinor))
            {
                if (ilr.LearnStartDate <= version.VersionLatestStartDate || version.VersionLatestStartDate == null)
                {
                    return version;
                }
            }

            return standard;
        }
    }
}
