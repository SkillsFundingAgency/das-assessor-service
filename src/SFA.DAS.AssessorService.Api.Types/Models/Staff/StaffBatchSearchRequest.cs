using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Staff
{
    public class StaffBatchSearchRequest : IRequest<StaffBatchSearchResponse>
    {
        public StaffBatchSearchRequest(int batchNumber, int page)
        {
            BatchNumber = batchNumber;
            Page = page;
        }

        public int BatchNumber { get; set; }
        public int Page { get; set; }
    }
}
