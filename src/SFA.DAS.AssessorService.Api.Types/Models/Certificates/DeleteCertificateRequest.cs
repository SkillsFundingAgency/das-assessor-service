using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class DeleteCertificateRequest : IRequest<Unit>
    {
        public long Uln { get; set; }
        public string UserName { get; set; }
        public int StandardCode { get; set; }
        public string ReasonForChange { get; set; }
        public string IncidentNumber { get; set; }
    }
}
