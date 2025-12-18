using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using WebShopMercantec.Services;


namespace WebShopMercantec.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthContoller : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDto rq)
        {
            var loginValue = rq.Username?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(loginValue))
                return BadRequest("No username value provided.");


            return Ok(new AuthResponse
            {

            });
        }

    } 
}
