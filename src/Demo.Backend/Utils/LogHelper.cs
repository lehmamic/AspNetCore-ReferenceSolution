using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Demo.Backend.Utils
{
    public static class LogHelper
    {
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            // Set the version attributes
            var assembly = Assembly.GetExecutingAssembly();
            string? informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            var assemblyVersion = assembly.GetName().Version?.ToString();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            diagnosticContext.Set("InformationalVersion", informationalVersion);
            diagnosticContext.Set("AssemblyVersion", assemblyVersion);
            diagnosticContext.Set("AssemblyFileVersion", fileVersionInfo.FileVersion);

            HttpRequest request = httpContext.Request;

            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);
            diagnosticContext.Set("HttpMode", "INBOUND");

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            Endpoint endpoint = httpContext.GetEndpoint();
            if (endpoint != null)
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }
    }
}