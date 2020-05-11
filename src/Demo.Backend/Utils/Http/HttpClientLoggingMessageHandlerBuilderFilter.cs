using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Http;

namespace Demo.Backend.Utils.Http
{
    internal class HttpClientLoggingMessageHandlerBuilderFilter : IHttpMessageHandlerBuilderFilter
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public HttpClientLoggingMessageHandlerBuilderFilter(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
        {
            return builder =>
            {
                builder.AdditionalHandlers.Add(new HttpClientLoggingMessageHandler(_contextAccessor));
                next(builder);
            };
        }
    }
}