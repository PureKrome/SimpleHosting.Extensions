using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TestWebApplication.Repositories;
using WorldDomination.SimpleHosting.Extensions;

namespace TestWebApplication
{
    public class Startup
    {
        private const string OpenApiVersion = "v2";
        private const string OpenApiTitle = "Test API";

        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesSwaggerSimple(services);
            //ConfigureServicesSwaggerComplex(services);

            ConfigureRepositories(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            ConfigureSwaggerSimple(app);
        }

        // TODO: Replace this with proper Integration testing overrides.
        //       REF: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.1#basic-tests-with-the-default-webapplicationfactory
        // This method can be overwritten in a test project to define our repository.
        //          You would not do this in production, but we need to test our Helper library!
        public virtual void ConfigureRepositories(IServiceCollection services)
        {
            // Create our fake data.
            var stubbedFakeVehicleRepository = StubbedFakeVehicleRepository.CreateAFakeVehicleRepository();

            // Inject our repo.
            services.AddSingleton<IFakeVehicleRepository>(stubbedFakeVehicleRepository);
        }

        // Format: <Controller>_<HTTP Method>_<MethodName>_<Guid>
        // E.g. : Home_GET_SearchAsync_e0ded733-11be-4dce-a3d9-ee8483719c4f
        // Having a randomized GUID for an operationId isn't a good idea. Each time the webserver starts,
        // a new ID is gerenated, which could ruin things for consumers requiring a consistent OperationId.
        // It is here as an example of customising the default behavior to prove that customisation works.
        private string CustomOperationIdSelector(ApiDescription apiDescription)
        {
            var controllerName = ((ControllerActionDescriptor)apiDescription.ActionDescriptor).ControllerName;
            var methodName = apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : $"Unknown_Method_Name_{Guid.NewGuid()}";
            return $"{controllerName}_{apiDescription.HttpMethod}_{methodName}_{Guid.NewGuid()}"; // Guid is for custom testing purposes.
        }

        private void ConfigureServicesSwaggerSimple(IServiceCollection services)
        {
            services.AddDefaultWebApiSettings();
        }

        public void ConfigureSwaggerSimple(IApplicationBuilder app)
        {
            app.UseDefaultWebApiSettings();
        }

        private void ConfigureServicesSwaggerComplex(IServiceCollection services)
        {
            var isStackTraceDisplayed = _webHostEnvironment.IsDevelopment();
            var isJsonIndented = _webHostEnvironment.IsDevelopment();
            string jsonDateTimeFormat = null;

            // Just mucking around - testing that we can use some custom stuff.
            var customSwaggerOptions = new Action<SwaggerGenOptions>(setupAction =>
            {
                setupAction.CustomOperationIds(CustomOperationIdSelector);

                var info = new OpenApiInfo
                {
                    Title = OpenApiTitle,
                    Version = OpenApiVersion
                };

                setupAction.SwaggerDoc(OpenApiVersion, info);
            });

            services.AddDefaultWebApiSettings("Some Test Api",
                                              isStackTraceDisplayed,
                                              isJsonIndented,
                                              jsonDateTimeFormat,
                                              customSwaggerOptions);


            ConfigureRepositories(services);
        }

        public void ConfigureSwaggerComplex(IApplicationBuilder app)
        {
            app.UseDefaultWebApiSettings(OpenApiTitle, OpenApiVersion);
        }
    }
}
