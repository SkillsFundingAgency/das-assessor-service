using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Middleware
{

    public class SandboxHeadersMiddleware
    {
        private const string _UserEmailHeader = "x-request-context-user-email";
        private const string _UserEmailHeaderValue = "sandbox@test.test";
        private const string _UserNoteHeader = "x-request-context-user-note";
        private const string _UserNoteHeaderValue = "ukprn=99999999";

        private readonly RequestDelegate _next;

        public SandboxHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.Headers.TryGetValue(_UserEmailHeader, out var emailHeaderValue);
            if (string.IsNullOrEmpty(emailHeaderValue))
            {
                context.Request.Headers[_UserEmailHeader] = _UserEmailHeaderValue;
            }
            context.Request.Headers[_UserNoteHeader] = _UserNoteHeaderValue;
            await _next(context);
        }
    }
}
