using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch
{
    public class GetFrameworkCertificateQuery: IRequest<GetFrameworkCertificateResult>
    {
        public Guid Id { get; set; }
    }
}
