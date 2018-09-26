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
        private readonly RequestDelegate _next;

        public GetHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IHeaderInfo headerInfo)
        {
            context.Request.Headers.TryGetValue("x-request-context-user-email", out var emailHeaderValue);
            context.Request.Headers.TryGetValue("x-request-context-user-ukprn", out var ukprnHeaderValue);

            bool validUkPrn = int.TryParse(ukprnHeaderValue.FirstOrDefault(), out int ukprn);
            string email = emailHeaderValue.FirstOrDefault() ?? string.Empty;

            if (validUkPrn && !string.IsNullOrWhiteSpace(email))
            {
                headerInfo.Ukprn = ukprn;
                headerInfo.Email = email;

                await _next(context);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var response = new ApiResponse(context.Response.StatusCode, "Invalid Headers");
                var json = JsonConvert.SerializeObject(response,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                await context.Response.WriteAsync(json);
            }
        }
    }
}