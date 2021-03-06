using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using WorldDomination.SimpleHosting.Extensions.Controllers;
using WorldDomination.SimpleHosting.Extensions.Models;

namespace WorldDomination.SimpleHosting.Extensions
{
    public static class IMvcBuilderExtensions
    {
        /// <summary>
        /// Registers a common webapi home controller.
        /// </summary>
        /// <param name="builder">An interface for configuring essential MVC services.</param>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        /// <param name="callingType">Type of the calling assembly. This would usually be the main assembly of the web site.</param>
        /// <param name="asciiBanner">Optional: some text to display in the home controller output.</param>
        /// <returns>Chaining: the interface for configuring essential MVC services.</returns>
        public static IMvcBuilder AddAHomeController(this IMvcBuilder builder,
                                                     IServiceCollection services,
                                                     string asciiBanner = null,
                                                     Assembly callingAssembly = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (callingAssembly == null)
            {
                callingAssembly = Assembly.GetCallingAssembly();
            }

            var banner = new HomeControllerBanner(asciiBanner, callingAssembly);
            services.AddSingleton<IHomeControllerBanner>(banner);

            builder.AddApplicationPart(typeof(HomeController).Assembly);

            return builder;
        }

        /// <summary>
        /// Sets up some common, default JsonSerializerOptions settings:<br/>
        /// - CamelCase property names.
        /// - Indented formatting.
        /// - Ignore null properties which have values.
        /// - Enums are rendered as string's ... not their backing number-value.
        /// </summary>
        /// <param name="builder"Mvc builder to help create the Mvc settings.></param>
        /// <returns>IMvcBuilder: the builder, so we can more builder methods.</returns>
        public static IMvcBuilder AddDefaultJsonOptions(this IMvcBuilder builder,
                                                        bool isIndented = false,
                                                        string dateTimeFormat = null,
                                                        IEnumerable<JsonConverter> converters = null)
        {
            return builder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
                options.JsonSerializerOptions.WriteIndented = isIndented;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                // If we have specified a custom date-time format, then use the specific custom converter.
                if (!string.IsNullOrWhiteSpace(dateTimeFormat))
                {
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter(dateTimeFormat));
                }

                if (converters?.Any() == true)
                {
                    foreach(var converter in converters)
                    {
                        options.JsonSerializerOptions.Converters.Add(converter);
                    }
                }
            });
        }
    }
}
