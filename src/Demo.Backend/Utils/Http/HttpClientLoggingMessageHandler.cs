using System;
using System.Net.Http;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;

namespace Demo.Backend.Utils.Http
{
    public class HttpClientLoggingMessageHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HttpClientLoggingMessageHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (_contextAccessor.HttpContext != null)
            {
                CorrelationContext correlationContext = _contextAccessor.HttpContext.RequestServices
                    .GetRequiredService<ICorrelationContextAccessor>()
                    .CorrelationContext;

                request.Headers.TryAddWithoutValidation("CorrelationId", correlationContext.CorrelationId);

                using IDisposable property = LogContext.PushProperty("HttpMode", "OUTBOUND");
                return await base.SendAsync(request, cancellationToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}