using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Application.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                if (ex is ApplicationException || ex is BadRequestException)
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                else if (ex is ResourceNotFoundException)
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                else if (ex is UnauthorisedException)
                    context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                else
                    context.Response.StatusCode = 500;

                _logger.LogError($"Unhandled Exeption raised : {ex.Message} : Stack Trace : {ex.StackTrace}");

                context.Response.ContentType = "application/json";

                var response = new ApiResponse(context.Response.StatusCode, ex.Message);
                var json = JsonConvert.SerializeObject(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}