using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Api.Types.Models
{

    public interface ISignInService
    {
        Task<InviteUserResponse> InviteUser(string email, string givenName, string familyName, Guid userId);
        Task<InviteUserResponse> InviteUserToOrganisation(string email, string givenName, string familyName, Guid userId, string organisationName, string inviter, Guid inviterId);
    }
}
