using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetContactBySignInIdRequest : IRequest<EpaContact>
    {
        public string SignInId { get; set; }
       
    }
}