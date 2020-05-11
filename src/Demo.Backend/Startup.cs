using System;
using System.Net.Http.Headers;
using AutoMapper;
using CorrelationId;
using Demo.Backend.RebusTest;
using Demo.Backend.SwisscomOpenApis;
using Demo.Backend.Utils;
using Demo.Backend.Utils.Http;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Rebus.Config;
using Rebus.DataBus.InMem;
using Rebus.Persistence.InMem;
using Rebus.Routing.TypeBased;
using Rebus.ServiceProvider;
using Rebus.Transport.InMem;
using Refit;
using Serilog;

namespace Demo.Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SwisscomOpenDataApiOptions>(Configuration.GetSection("SwisscomOpenDataApi"));

            services.AddHttpClient("Swisscom", (serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<SwisscomOpenDataApiOptions>>();
                client.BaseAddress = new Uri(options.Value.BaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddTypedClient(RestService.For<ISwisscomOpenDataApi>)
            .AddHttpClientLogging()
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            services.AddRebus((configure, serviceProvider) => configure
                .Logging(l => l.Serilog())
                .Transport(t => t.UseInMemoryTransport(new InMemNetwork(), "Demo.Messages"))
                .Subscriptions(s => s.StoreInMemory(new InMemorySubscriberStore()))
                .DataBus(b => b.StoreInMemory(new InMemDataStore()))
                .Routing(c => c.TypeBased().MapAssemblyOf<TestCommand>("Target.Messages"))
                .Events(e => RebusHelper.EnrichCorrelationId(e, serviceProvider)));

            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidatorActionFilter));
            }).AddFluentValidation(fvc =>
            {
                fvc.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

            services.AddCorrelationId();
            services.AddHttpContextAccessor();
            services.AddHealthChecks();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = "CorrelationId",
                UpdateTraceIdentifier = true,
                IncludeInResponse = true,
                UseGuidForCorrelationId = true,
            });
            app.UseCorrelationIdLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
            {
                builder.UseSpa(spa => { });
            });
        }
    }
}