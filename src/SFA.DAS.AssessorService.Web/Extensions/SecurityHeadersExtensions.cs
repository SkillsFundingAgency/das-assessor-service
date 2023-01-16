using Microsoft.AspNetCore.Builder;

namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                const string dasCdn = "das-at-frnt-end.azureedge.net das-pp-frnt-end.azureedge.net das-mo-frnt-end.azureedge.net das-test-frnt-end.azureedge.net das-test2-frnt-end.azureedge.net das-prd-frnt-end.azureedge.net";

                context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["Content-Security-Policy"] =
                        $"script-src 'self' 'unsafe-inline' 'unsafe-eval' { dasCdn} https://www.googletagmanager.com https://tagmanager.google.com https://www.google-analytics.com https://ssl.google-analytics.com https://*.zdassets.com https://*.zopim.com https://*.rcrsv.io; " +
                        $"style-src 'self' 'unsafe-inline' {dasCdn} https://tagmanager.google.com https://fonts.googleapis.com https://*.rcrsv.io ; " +
                        $"img-src {dasCdn} www.googletagmanager.com https://ssl.gstatic.com https://www.gstatic.com https://www.google-analytics.com ; " +
                        $"font-src {dasCdn} https://fonts.gstatic.com https://*.rcrsv.io data: ;" +
                        "connect-src https://www.google-analytics.com https://*.zendesk.com https://*.zdassets.com wss://*.zopim.com https://*.rcrsv.io ;";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                await next();
            });

            return app;
        }
    }
}