using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners
{
    public class GetBatchLearnerRequest : IRequest<GetBatchLearnerResponse>
    {
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public string Standard { get; set; }

        public bool IncludeCertificate { get; set; }

        public int UkPrn { get; set; }
        public string Email { get; set; }
    }
}
