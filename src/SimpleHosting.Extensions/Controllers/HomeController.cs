using Microsoft.AspNetCore.Mvc;
using System;
using WorldDomination.SimpleHosting.Extensions.Models;

namespace WorldDomination.SimpleHosting.Extensions.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeControllerBanner _banner;

        public HomeController(IHomeControllerBanner banner)
        {
            _banner = banner ?? throw new ArgumentNullException(nameof(banner));
        }

        /// <summary>
        /// Main Api home page. Just  means the API is up and running.
        /// </summary>
        /// <returns>Some health-related text.</returns>
        /// <response code="200">Some text saying "yep, we're up!".</response>
        [HttpGet("")]
        public OkObjectResult Get()
        {
            return Ok(_banner.Banner);
        }

        [HttpGet("exceptionTest")]
        [ProducesResponseType(500)]
        public ActionResult ExceptionTest()
        {
            throw new Exception($"Testing exceptions. {DateTime.UtcNow}");
        }
    }
}
