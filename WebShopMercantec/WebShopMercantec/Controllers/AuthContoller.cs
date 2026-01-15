using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using WebShopMercantec.Models;
using WebShopMercantec.Services;


namespace WebShopMercantec.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthContoller : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwt;

        public AuthContoller(IUserService userService, IJwtService JwtService)
        {
            _userService = userService;
            _jwt = JwtService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginDto rq)
        {
            var loginValue = rq.Username?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(loginValue))
                return BadRequest("No username value provided.");

            var user = await _userService.GetUserByLogin(loginValue);
            if (user == null)
                return BadRequest("Wrong username/email or password.");

            if (!BCrypt.Net.BCrypt.Verify(rq.Password, user.Password))
                return BadRequest(".");

            var token = _jwt.GenerateToken(user, user.Role);

            return Ok(new AuthResponse
            {
                AccessToken = token,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(60),
                Message = "Login successful"
            });
        }

    } 
}
