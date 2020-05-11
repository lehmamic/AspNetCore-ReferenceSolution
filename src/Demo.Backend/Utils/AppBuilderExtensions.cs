using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;

namespace Demo.Backend.Utils
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseCorrelationIdLogging(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                string correlationId = app.ApplicationServices.GetRequiredService<ICorrelationContextAccessor>()
                    .CorrelationContext
                    .CorrelationId;

                using (LogContext.PushProperty("CorrelationId", correlationId))
                {
                    await next();
                }
            });

            return app;
        }
    }
}