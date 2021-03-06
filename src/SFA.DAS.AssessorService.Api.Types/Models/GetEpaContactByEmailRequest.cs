using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEpaContactByEmailRequest : IRequest<EpaContact>
    {
        public string Email { get; set; }
    }
}