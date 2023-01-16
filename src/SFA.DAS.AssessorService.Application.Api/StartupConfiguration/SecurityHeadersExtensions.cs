using Microsoft.AspNetCore.Builder;

namespace SFA.DAS.AssessorService.Application.Api.StartupConfiguration
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
                context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; img-src 'self' *.azureedge.net *.google-analytics.com; script-src 'self' 'unsafe-inline' *.azureedge.net *.googletagmanager.com *.google-analytics.com *.googleapis.com; style-src-elem 'self' *.azureedge.net; style-src 'self' *.azureedge.net; font-src 'self' *.azureedge.net data:;";
                context.Response.Headers["Referrer-Policy"] = "strict-origin";
                context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                context.Response.Headers["Pragma"] = "no-cache";
                await next();
            });
            
            return app;
        }
    }
}