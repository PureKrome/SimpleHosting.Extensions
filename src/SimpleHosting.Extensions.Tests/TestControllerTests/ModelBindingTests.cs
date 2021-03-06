using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System;
using System.Net;
using System.Threading.Tasks;
using TestWebApplication.Models;
using Xunit;

namespace WorldDomination.SimpleHosting.Extensions.Tests.TestControllerTests
{
    public class ModelBindingTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _factory;

        public ModelBindingTests(TestFixture factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        [Fact]
        public async Task GivenAValidModelBind_Get_ReturnsAnHttp200()
        {
            // Arrange & Act.
            var response = await _factory.CreateClient().GetAsync($"/test/modelbinding/{ColourType.GreenAndPink}");

            // Assert.
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GivenABadModelBind_Get_ReturnsAnHttp400()
        {
            // Arrange.
            var error = new ValidationProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Please refer to the errors property for additional details.",
                Instance = "/test/modelbinding/pewpew"
            };
            error.Errors.Add("colour", new[] { "The value 'pewpew' is not valid." });

            // Act.
            var response = await _factory.CreateClient().GetAsync($"/test/modelbinding/pewpew");

            // Assert.
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            await response.Content.ShouldHaveSameProblemDetails(error);
        }
    }
}
