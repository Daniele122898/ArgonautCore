
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArgonautCore.Network.Authentication.Test.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult TestNoAuthentication()
        {
            return Ok();
        }

        [HttpGet("auth")]
        [Authorize]
        public IActionResult TestWithAuthentication()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            string username = User.FindFirst(ClaimTypes.Name).Value;
            return Ok(new {userId, username});
        }
    }
}