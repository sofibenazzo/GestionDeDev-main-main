using GestionDeDevoluciones.Backend.Auth;
using GestionDeDevoluciones.Backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GestionDeDevoluciones.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDTO dto)
        {
            var token = _authService.Register(dto);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {
            var token = _authService.Login(dto);
            return Ok(new { token });
        }
    }
}
