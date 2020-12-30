using Microsoft.AspNetCore.Mvc;
using Users.Business.Contracts;
using Users.Business.Interfaces;

namespace Users.Api.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost, Route("api/login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var token = _loginService.Authenticate(loginDto);

            return Ok(token);
        }

        [HttpPost, Route("api/validate")]
        public IActionResult Validate([FromBody] string token)
        {
            var user = _loginService.ValidateToken(token);

            return Ok(user);
        }
    }
}
