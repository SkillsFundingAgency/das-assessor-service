using System;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public interface IAzureTokenService
    {
        string CreateSharedAccessToken(DateTime expiry);
    }
}
