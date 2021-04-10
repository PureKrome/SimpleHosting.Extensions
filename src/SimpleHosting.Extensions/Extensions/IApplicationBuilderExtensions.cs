using System;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;
using WorldDomination.SimpleHosting.Extensions.Models;

namespace WorldDomination.SimpleHosting.Extensions
{
    public static class IApplicationBuilderExtensions
    {

        public static IApplicationBuilder UseCustomOpenApi(this IApplicationBuilder application,
            OpenApiSettings openApiSettings)
        {
            if (openApiSettings is null)
            {
                throw new ArgumentNullException(nameof(openApiSettings));
            }

            application.UseSwagger();
            
            application.UseSwaggerUI(c =>
            {
                // e.g. /swagger/v1/swagger.json
                c.SwaggerEndpoint($"/{openApiSettings.RoutePrefix}/{openApiSettings.Version}/swagger.json", openApiSettings.Title);
                c.DisplayOperationId();
            });

            return application;
        }

        /// <summary>
        /// This menthod uses the common Web Api settings:<br/>
        /// - UseProblemDetails<br/>
        /// - UseCustomOpenApi<br/>
        /// - UseRouting<br/>
        /// - UseAuthorization<br/>
        /// - UseEndpoints that map controllers<br/>
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultWebApiSettings(this IApplicationBuilder application)
        {
            return application.UseDefaultWebApiSettings(OpenApiSettings.DefaultOpenApiTitle,
                OpenApiSettings.DefaultOpenApiVersion,
                OpenApiSettings.DefaultOpenApiRoutePrefex);
        }

        /// <summary>
        /// This menthod uses the common Web Api settings:<br/>
        /// - UseProblemDetails<br/>
        /// - UseCustomOpenApi<br/>
        /// - UseRouting<br/>
        /// - OPTIONAL: UseAuthorization<br/>
        /// - UseEndpoints that map controllers<br/>
        /// </summary>
        /// <param name="application"></param>
        /// <param name="title"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultWebApiSettings(this IApplicationBuilder application,
            string title,
            string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException(nameof(version));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(nameof(title));
            }

            return application.UseDefaultWebApiSettings(title, version, OpenApiSettings.DefaultOpenApiRoutePrefex);
        }

        /// <summary>
        /// This menthod uses the common Web Api settings:<br/>
        /// - UseProblemDetails<br/>
        /// - UseCustomOpenApi<br/>
        /// - UseRouting<br/>
        /// - OPTIONAL: UseAuthorization<br/>
        /// - UseEndpoints that map controllers<br/>
        /// </summary>
        /// <param name="application"></param>
        /// <param name="useAuthorization"></param>
        /// <param name="title"></param>
        /// <param name="version"></param>
        /// <param name="routePrefix"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultWebApiSettings(this IApplicationBuilder application,
            string title,
            string version,
            string routePrefix,
            bool useAuthorization = true)
        {
            if (string.IsNullOrWhiteSpace(routePrefix))
            {
                throw new ArgumentException(nameof(routePrefix));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException(nameof(version));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(nameof(title));
            }

            OpenApiSettings openApiSettings = new()
            {
                Title = title,
                Version = version,
                RoutePrefix = routePrefix
            };

            return application.UseDefaultWebApiSettings(useAuthorization, openApiSettings);
        }

        /// <summary>
        /// This menthod uses the common Web Api settings:<br/>
        /// - UseProblemDetails<br/>
        /// - UseCustomOpenApi<br/>
        /// - UseRouting<br/>
        /// - OPTIONAL: UseAuthorization<br/>
        /// - UseEndpoints that map controllers<br/>
        /// </summary>
        /// <param name="application"></param>
        /// <param name="useAuthorization"></param>
        /// <param name="openApiSettings"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDefaultWebApiSettings(this IApplicationBuilder application,
            bool useAuthorization = true,
            OpenApiSettings openApiSettings = null)
        {
            // NOTE: the CustomOpenApi could be optional. 
            //       if Development, then show. Otherwise, don't show/enable.
            application.UseProblemDetails()
                       .UseRouting();

            // People might not want to show this, if it's for production, etc.
            if (openApiSettings != null)
            {
                application.UseCustomOpenApi(openApiSettings);
            }

            if (useAuthorization)
            {
                application.UseAuthorization();
            }

            application.UseEndpoints(endpoints => endpoints.MapControllers());

            return application;
        }
    }
}
