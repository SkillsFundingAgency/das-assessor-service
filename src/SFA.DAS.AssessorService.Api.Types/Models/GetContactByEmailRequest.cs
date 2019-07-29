using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetContactByEmailRequest : IRequest<EpaContact>
    {
        public string Email { get; set; }
       
    }
}