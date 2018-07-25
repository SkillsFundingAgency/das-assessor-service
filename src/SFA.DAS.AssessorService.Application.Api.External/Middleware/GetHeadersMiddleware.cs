using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.AssessorService.Application.Api.External.Middleware
{
    public class GetHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public GetHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IHeaderInfo headerInfo)
        {
            context.Request.Headers.TryGetValue("x-username", out var usernameHeaderValue);
            context.Request.Headers.TryGetValue("x-ukprn", out var ukprnHeaderValue);

            int.TryParse(ukprnHeaderValue.FirstOrDefault(), out int ukprn);
            string username = usernameHeaderValue.FirstOrDefault() ?? string.Empty;

            headerInfo.Ukprn = ukprn;
            headerInfo.Username = username;

            await _next(context);
        }
    }
}