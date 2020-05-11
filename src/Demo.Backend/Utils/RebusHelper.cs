using System;
using CorrelationId;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Serilog.Context;

namespace Demo.Backend.Utils
{
    public static class RebusHelper
    {
        public static void EnrichCorrelationId(EventConfigurer e, IServiceProvider serviceProvider)
        {
            e.BeforeMessageSent += (bus, headers, message, context) =>
            {
                CorrelationContext correlationContext = serviceProvider
                    .GetRequiredService<ICorrelationContextAccessor>()
                    .CorrelationContext;

                if (correlationContext == null)
                {
                    correlationContext = serviceProvider.GetRequiredService<ICorrelationContextFactory>()
                        .Create(Guid.NewGuid().ToString(), "CorrelationId");
                }

                headers.Add("rbs2-corr-id", correlationContext.CorrelationId);
            };

            e.BeforeMessageHandled += (bus, headers, message, context, args) =>
            {
                if (headers.TryGetValue("rbs2-corr-id", out string? correlationId))
                {
                    IDisposable correlationIdProperty = LogContext.PushProperty("CorrelationId", correlationId);
                    context.Save("CorrelationIdLogProperty", correlationIdProperty);

                    CorrelationContext correlationContext = serviceProvider
                        .GetRequiredService<ICorrelationContextAccessor>()
                        .CorrelationContext;

                    if (correlationContext == null)
                    {
                        serviceProvider.GetRequiredService<ICorrelationContextFactory>()
                            .Create(correlationId, "CorrelationId");
                    }
                }
            };

            e.AfterMessageHandled += (bus, headers, message, context, args) =>
            {
                context.Load<IDisposable>("CorrelationIdLogProperty")?.Dispose();
            };
        }
    }
}