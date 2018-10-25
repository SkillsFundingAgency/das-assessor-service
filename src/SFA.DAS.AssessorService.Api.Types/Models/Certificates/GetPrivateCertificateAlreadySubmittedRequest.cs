using System;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GetPrivateCertificateAlreadySubmittedRequest : IRequest<GetPrivateCertificateAlreadySubmittedResponse>
    {
        public Guid Id { get; set; }            
    }
}