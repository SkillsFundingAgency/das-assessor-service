using System;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEpaContactRequest : IRequest<EpaContact>
    {
        public Guid ContactId { get; set; }
    }
}