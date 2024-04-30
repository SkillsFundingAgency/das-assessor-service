using Polly;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Api.Common
{
    public static class PollyUtils
    {
        public static ValueTask<bool> HandleTransientHttpError(
            Outcome<HttpResponseMessage> outcome) =>
            outcome switch
            {
                { Exception: HttpRequestException } => PredicateResult.True(),
                { Result.StatusCode: HttpStatusCode.RequestTimeout } => PredicateResult.True(),
                { Result.StatusCode: HttpStatusCode.InternalServerError } => PredicateResult.True(),
                _ => PredicateResult.False()
            };
    }
}
