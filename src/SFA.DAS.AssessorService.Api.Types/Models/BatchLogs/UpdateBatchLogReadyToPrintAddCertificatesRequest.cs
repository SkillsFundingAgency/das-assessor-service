using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class UpdateBatchLogReadyToPrintAddCertificatesRequest : IRequest<int>
    {
        public int BatchNumber { get; set; }
        public int MaxCertificatesToBeAdded { get; set; }
    }
}
