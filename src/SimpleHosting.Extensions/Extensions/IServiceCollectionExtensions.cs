using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WorldDomination.SimpleHosting.Extensions
{
    public static class IServiceCollectionExtensions
    {
        private const string DefaultOpenApiTitle = "My API";
        private const string DefaultOpenApiVersion = "v1";

        public static IServiceCollection AddDefaultWebApiSettings(this IServiceCollection services)
        {
            return services.AddDefaultWebApiSettings(default,
                default,
                default,
                default,
                DefaultOpenApiTitle,
                DefaultOpenApiVersion,
                default);
        }

        ///// <summary>
        ///// This method adds the common web api services:<br/>
        ///// - AddControllers<br/>
        ///// - AddHomeController<br/>
        ///// - AddDefaultJsonOptions<br/>
        ///// - AddProblemDeatils<br/>
        ///// - AddCustomSwagger<br/>
        ///// </summary>
        ///// <param name="services"></param>
        ///// <param name="banner"></param>
        ///// <param name="includeExceptionDetails"></param>
        ///// <param name="isJsonIndented"></param>
        ///// <returns></returns>
        //public static IServiceCollection AddDefaultWebApiSettings(this IServiceCollection services,
        //                                                          string banner = null,
        //                                                          bool includeExceptionDetails = false,
        //                                                          bool isJsonIndented = false)
        //{
        //    return services.AddDefaultWebApiSettings(banner,
        //        includeExceptionDetails,
        //        isJsonIndented,
        //        default,
        //        default,
        //        default,
        //        default);
        //}

        /// <summary>
        /// This method adds the common web api services:<br/>
        /// - AddControllers<br/>
        /// - AddHomeController<br/>
        /// - AddDefaultJsonOptions<br/>
        /// - AddProblemDeatils<br/>
        /// - AddCustomSwagger<br/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="banner"></param>
        /// <param name="includeExceptionDetails"></param>
        /// <param name="isJsonIndented"></param>
        /// <param name="jsonDateTimeFormat"></param>
        /// <param name="title"></param>
        /// <param name="version"></param>
        /// <param name="otherChainedMethods"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultWebApiSettings(this IServiceCollection services,
                                                                  string banner = null,
                                                                  bool includeExceptionDetails = false,
                                                                  bool isJsonIndented = false,
                                                                  string jsonDateTimeFormat = null,
                                                                  string title = DefaultOpenApiTitle,
                                                                  string version = DefaultOpenApiVersion,
                                                                  IEnumerable<Action<IMvcBuilder>> otherChainedMethods = null)
        {

            var swaggerGenerationOptions = services.CreateCustomOpenApi(title, version);

            return services.AddDefaultWebApiSettings(banner,
                includeExceptionDetails,
                isJsonIndented,
                jsonDateTimeFormat,
                swaggerGenerationOptions,
                otherChainedMethods);
        }

        /// <summary>
        /// This method adds the common web api services:<br/>
        /// - AddControllers<br/>
        /// - AddHomeController<br/>
        /// - AddDefaultJsonOptions<br/>
        /// - AddProblemDeatils<br/>
        /// - AddCustomSwagger<br/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="banner"></param>
        /// <param name="includeExceptionDetails"></param>
        /// <param name="isJsonIndented"></param>
        /// <param name="jsonDateTimeFormat"></param>
        /// <param name="customSwaggerGenerationOptions"></param>
        /// <param name="otherChainedMethods"></param>
        /// <returns></returns>
        public static IServiceCollection AddDefaultWebApiSettings(this IServiceCollection services,
            string banner = null,
            bool includeExceptionDetails = false,
            bool isJsonIndented = false,
            string jsonDateTimeFormat = null,
            Action<SwaggerGenOptions> customSwaggerGenerationOptions = null,
            IEnumerable<Action<IMvcBuilder>> otherChainedMethods = null)
        {
            var mvcBuilder = services.AddControllers();

            mvcBuilder
                .AddAHomeController(services, banner, Assembly.GetCallingAssembly())
                .AddDefaultJsonOptions(isJsonIndented, jsonDateTimeFormat);

            services.AddProblemDetails(options =>
            {
                options.IncludeExceptionDetails = (ctx, ex) => includeExceptionDetails;
            });

            // Use the provided action OR use a default one.
            services.AddSwaggerGen(customSwaggerGenerationOptions ?? services.CreateCustomOpenApi());

            if (otherChainedMethods != null &&
                otherChainedMethods.Any())
            {
                foreach(var method in otherChainedMethods)
                {
                    method(mvcBuilder);
                }
            }

            return services;
        }

        private static Action<SwaggerGenOptions> CreateCustomOpenApi(this IServiceCollection services,
                                                                     string title = DefaultOpenApiTitle,
                                                                     string version = DefaultOpenApiVersion)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(nameof(title));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException(nameof(version));
            }

            return new Action<SwaggerGenOptions>(setupAction =>
            {
                // These are the Method Id's - each method needs to be unique.
                setupAction.CustomOperationIds(CustomOperationIdSelector);

                // These are the schema's of the models (e.g. the response model)
                // which also needs to be unique.
                setupAction.CustomSchemaIds(type =>
                {
                    var controllerName = type.DeclaringType == null
                        ? string.Empty
                        : $"{type.DeclaringType.Name}_";

                    return $"{controllerName}{type.Name}";
                });

                var info = new OpenApiInfo
                {
                    Title = title,
                    Version = version
                };

                setupAction.SwaggerDoc(version, info);
            });
        }

        // Format: <Controller>_<HTTP Method>_<MethodName>
        // E.g. : Home_GET_SearchAsync
        private static string CustomOperationIdSelector(ApiDescription apiDescription)
        {
            var controllerName = ((ControllerActionDescriptor)apiDescription.ActionDescriptor).ControllerName;
            var methodName = apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : $"Unknown_Method_Name_{Guid.NewGuid()}";
            return $"{controllerName}_{apiDescription.HttpMethod}_{methodName}";
        }
    }
}
