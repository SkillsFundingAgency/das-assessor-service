using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class CertificateAddressResponse : IRequest
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
    }
}
