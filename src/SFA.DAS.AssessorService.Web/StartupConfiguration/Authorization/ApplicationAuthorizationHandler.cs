using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.Services;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.StartupConfiguration
{
    public class ApplicationAuthorizationHandler : AuthorizationHandler<ApplicationRequirement>
    {
        private readonly IApplicationApiClient _applicationApiClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClaimService _claimService;
        private readonly ILogger<ApplicationAuthorizationHandler> _logger;

        public ApplicationAuthorizationHandler(IApplicationApiClient applicationApiClient, IHttpContextAccessor httpContextAccessor,
            IClaimService claimService, ILogger<ApplicationAuthorizationHandler> logger)
        {
            _applicationApiClient = applicationApiClient;
            _httpContextAccessor = httpContextAccessor;
            _claimService = claimService;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApplicationRequirement requirement)
        {
            var userId = _claimService.UserId;
            if (userId.HasValue)
            {
                var routeId = _httpContextAccessor.HttpContext.GetRouteValue(requirement.RouteId) as string
                    ?? _httpContextAccessor.HttpContext.Request.Query[requirement.RouteId][0];

                if (Guid.TryParse(routeId, out Guid id))
                {
                    try
                    {
                        if (await _applicationApiClient.GetApplicationForUser(id, userId.Value) != null)
                        {
                            context.Succeed(requirement);
                        }
                    }
                    catch
                    {
                        _logger.LogError($"Attempt to access application {id} by user {userId.Value} was not allowed.");
                        context.Fail();
                    }
                }
            }
        }
    }
}