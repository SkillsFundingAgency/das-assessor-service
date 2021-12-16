using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class CertificateSessionExtensions
    {
        private const string RedirectToCheck = nameof(RedirectToCheck);

        public static bool GetRedirectToCheck(this ISessionService sessionService)
        {
            return sessionService.TryGet<bool>(RedirectToCheck, out var redirectToCheck) ? redirectToCheck : false;
        }

        public static void SetRedirectToCheck(this ISessionService sessionService, bool value)
        {
            sessionService.Set(RedirectToCheck, value);
        }
    }
}
