using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Handlers.Staff
{
    public class LearnerDetailRequest : IRequest<LearnerDetail>
    {
        public string  CertificateReference { get; set; }
        public int StdCode { get; }
        public long Uln { get; }
        public bool AllRecords { get; }

        public LearnerDetailRequest(
            string certificateReference,
            int stdCode,
            long uln,
            bool allRecords
            )
        {
            CertificateReference = certificateReference;
            StdCode = stdCode;
            Uln = uln;
            AllRecords = allRecords;
        }
    }
}