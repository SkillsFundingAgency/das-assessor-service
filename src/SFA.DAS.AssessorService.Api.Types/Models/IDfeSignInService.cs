using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Api.Types.Models
{

    public interface IDfeSignInService
    {
        Task<InviteUserResponse> InviteUser(string email, string givenName, string familyName, Guid userId);
    }
}
