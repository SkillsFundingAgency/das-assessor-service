using Microsoft.AspNetCore.Http;

namespace SFA.DAS.AssessorService.Web.Helpers;

public static class EpaOrgIdFinder
{
    public static string ClaimType => "http://schemas.portal.com/epaoid";

    public static string GetFromClaim(IHttpContextAccessor accessor)
    {
        return accessor?.HttpContext?.User?.FindFirst(ClaimType)?.Value;
    }
}
