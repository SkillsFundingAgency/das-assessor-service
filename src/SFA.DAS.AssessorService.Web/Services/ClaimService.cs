using Microsoft.AspNetCore.Http;
using System;

namespace SFA.DAS.AssessorService.Web.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimService(IHttpContextAccessor httpContextAssessor)
        {
            _httpContextAccessor = httpContextAssessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(nameof(UserId));
                if(userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return userId;
                }

                return null;
            }
        }
    }
}
