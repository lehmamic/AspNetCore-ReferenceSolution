using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Serilog;
using System.Collections.Generic;
using System.Linq;

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

            // Log Request Headers
            Dictionary<string, StringValues> requestHeaders = request.Headers.ToDictionary(
                i => i.Key,
                i => i.Value);

            // remove sensitive Authorization header because it contains the JWT token with personal data
            requestHeaders.Remove("Authorization");
            diagnosticContext.Set("RequestHeaders", requestHeaders, true);

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

            HttpResponse response = httpContext.Response;

            // Set all the common properties available for every response

            // Log Response Headers
            Dictionary<string, StringValues> responseHeaders = request.Headers.ToDictionary(
                i => i.Key,
                i => i.Value);

            diagnosticContext.Set("ResponseHeaders", responseHeaders, true);
        }
    }
}