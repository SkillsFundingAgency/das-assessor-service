using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEpaContactBySignInIdRequest : IRequest<EpaContact>
    {
        public string SignInId { get; set; }
    }
}