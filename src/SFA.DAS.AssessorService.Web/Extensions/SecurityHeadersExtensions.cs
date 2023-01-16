using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Primitives;

namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                const string dasCdn = "das-at-frnt-end.azureedge.net das-pp-frnt-end.azureedge.net das-mo-frnt-end.azureedge.net das-test-frnt-end.azureedge.net das-test2-frnt-end.azureedge.net das-prd-frnt-end.azureedge.net";

                context.Response.Headers.AddIfNotPresent("x-frame-options", new StringValues("DENY"));
                context.Response.Headers.AddIfNotPresent("x-content-type-options", new StringValues("nosniff"));
                context.Response.Headers.AddIfNotPresent("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
                context.Response.Headers.AddIfNotPresent("x-xss-protection", new StringValues("0"));
                context.Response.Headers.AddIfNotPresent(
                    "Content-Security-Policy",
                    new StringValues(
                        $"script-src 'self' 'unsafe-inline' 'unsafe-eval' { dasCdn} https://www.googletagmanager.com https://tagmanager.google.com https://www.google-analytics.com https://ssl.google-analytics.com https://*.zdassets.com https://*.zopim.com https://*.rcrsv.io; " +
                        $"style-src 'self' 'unsafe-inline' {dasCdn} https://tagmanager.google.com https://fonts.googleapis.com https://*.rcrsv.io ; " +
                        $"img-src {dasCdn} www.googletagmanager.com https://ssl.gstatic.com https://www.gstatic.com https://www.google-analytics.com ; " +
                        $"font-src {dasCdn} https://fonts.gstatic.com https://*.rcrsv.io data: ;" +
                        "connect-src https://www.google-analytics.com https://*.zendesk.com https://*.zdassets.com wss://*.zopim.com https://*.rcrsv.io ;"));

                await next();
            });

            return app;
        }
    }
}