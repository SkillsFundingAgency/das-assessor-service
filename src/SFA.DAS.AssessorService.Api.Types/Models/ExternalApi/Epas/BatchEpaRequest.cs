using MediatR;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Epas
{
    public class BatchEpaRequest : IRequest<EpaDetails>
    {
        public string RequestId { get; set; }
        public long Uln { get; set; }
        public string FamilyName { get; set; }

        public int StandardCode { get; set; }
        public string StandardReference { get; set; }

        public int UkPrn { get; set; }

        public EpaDetails EpaDetails { get; set; }
    }
}
