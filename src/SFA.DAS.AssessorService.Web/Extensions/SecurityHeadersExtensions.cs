using Microsoft.AspNetCore.Builder;

namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self' 'unsafe-inline' https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com https://*.rcrsv.io; " +
                "img-src 'self' *.google-analytics.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com; " +
                "script-src 'self' 'unsafe-inline' " +
                "*.googletagmanager.com *.postcodeanywhere.co.uk *.google-analytics.com *.googleapis.com https://*.zdassets.com https://*.zendesk.com wss://*.zendesk.com wss://*.zopim.com https://*.rcrsv.io; " +
                "font-src 'self' data: https://*.rcrsv.io;";
                context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                await next();
            });

            return app;
        }
    }
}