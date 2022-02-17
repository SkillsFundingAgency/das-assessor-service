using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class CertificateSessionExtensions
    {
        private const string RedirectToCheck = nameof(RedirectToCheck);
        private const string RedirectedFromVersion = nameof(RedirectedFromVersion);

        public static bool GetRedirectToCheck(this ISessionService sessionService)
        {
            return sessionService.TryGet<bool>(RedirectToCheck, out var redirectToCheck) ? redirectToCheck : false;
        }

        public static void SetRedirectToCheck(this ISessionService sessionService, bool value)
        {
            sessionService.Set(RedirectToCheck, value);
        }

        public static bool GetRedirectedFromVersion(this ISessionService sessionService)
        {
            return sessionService.TryGet<bool>(RedirectedFromVersion, out var redirectedFromVersion) ? redirectedFromVersion : false;
        }

        public static void SetRedirectedFromVersion(this ISessionService sessionService, bool value)
        {
            sessionService.Set(RedirectedFromVersion, value);
        }

        public static void RemoveRedirectedFromVersion(this ISessionService sessionService)
        {
            sessionService.Remove(RedirectedFromVersion);
        }
    }
}
