using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEpaContactByGovUkIdentifierRequest : IRequest<EpaContact>
    {
        public string GovUkIdentifier { get; set; }
    }
}