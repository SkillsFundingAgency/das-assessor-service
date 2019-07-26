using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class UpdateCertificatesBatchNumberRequest : IRequest
    {
        public List<string> CertificateReference { get; set; }
        public int BatchNumber { get; set; }
    }
}
