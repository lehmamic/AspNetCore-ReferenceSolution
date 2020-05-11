using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace Demo.Backend.Utils.Http
{
    public static class HttpClientLoggingExtensions
    {
        public static IServiceCollection AddHttpClientLogging(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHttpMessageHandlerBuilderFilter, HttpClientLoggingMessageHandlerBuilderFilter>());
            return services;
        }

        public static IHttpClientBuilder AddHttpClientLogging(this IHttpClientBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.AddHttpMessageHandler(serviceProvider =>
            {
                var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

                return new HttpClientLoggingMessageHandler(contextAccessor);
            });

            return builder;
        }
    }
}