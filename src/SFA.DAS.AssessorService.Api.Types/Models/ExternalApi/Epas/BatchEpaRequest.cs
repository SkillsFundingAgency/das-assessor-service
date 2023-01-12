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
        public string Version { get; set; }
        public string CourseOption { get; set; }

        public int UkPrn { get; set; }

        public EpaDetails EpaDetails { get; set; }

        /// <summary>
        /// Internal ID, only gets populated on internal API if Version is supplied
        /// </summary>
        public string StandardUId { get; set; }
    }
}
