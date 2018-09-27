using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SFA.DAS.AssessorService.Application.Api.External.Middleware
{
    public class GetHeadersMiddleware
    {
        private const string _UserIdHeader = "x-request-context-user-id";
        private const string _UserEmailHeader = "x-request-context-user-email";
        private const string _UserNoteHeader = "x-request-context-user-note";
        private const string _InvalidUkprnMessage = "Your account is not linked to a valid UKPRN";
        private const string _InvalidEmailMessage = "Your account is not linked to a valid Email address";

        private readonly RequestDelegate _next;

        public GetHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IHeaderInfo headerInfo)
        {
            context.Request.Headers.TryGetValue(_UserEmailHeader, out var emailHeaderValue);
            context.Request.Headers.TryGetValue(_UserNoteHeader, out var noteHeaderValue);

            string email = emailHeaderValue.FirstOrDefault();

            if(!TryExtractUkprnFromHeader(noteHeaderValue, out var ukprn))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                var json = GetApiResponseAsJson(context.Response.StatusCode, _InvalidUkprnMessage);
                await context.Response.WriteAsync(json);
            }
            else if (string.IsNullOrWhiteSpace(email))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                var json = GetApiResponseAsJson(context.Response.StatusCode, _InvalidEmailMessage);
                await context.Response.WriteAsync(json);
            }
            else
            {
                headerInfo.Ukprn = ukprn;
                headerInfo.Email = email;

                await _next(context);
            }
        }

        private static bool TryExtractUkprnFromHeader(string header, out int ukprn)
        {
            ukprn = 0;

            if (string.IsNullOrWhiteSpace(header)) return false;

            string strippedValue = header.Trim().ToLower()
                                    .Replace("ukprn=", string.Empty);

            return int.TryParse(strippedValue, out ukprn);
        }

        private static string GetApiResponseAsJson(int statusCode, string message)
        {
            var apiResponse = new ApiResponse(statusCode, message);

            return JsonConvert.SerializeObject(apiResponse,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }
    }
}