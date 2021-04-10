using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestWebApplication.Models;
using TestWebApplication.Repositories;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestWebApplication.Controllers
{
    [AllowAnonymous]
    [Route("foo")]
    [ApiController]
    public class FooController : ControllerBase
    {
        // This model needs to exist in another controller.
        public class ResponseModel
        {
            public int Age { get; set; }
            public string Nickname { get; set; }
            public DateTime DateTime { get; } = DateTime.Now;
        }

        public FooController()
        {
        }

        [HttpGet("ModelTest")]
        [ProducesResponseType(typeof(ResponseModel), (int)HttpStatusCode.OK)]
        public IActionResult ModelTest()
        {
            var model = new ResponseModel
            {
                Age = 1,
                Nickname = "blah"
            };

            return Ok(model);
        }
    }
}
