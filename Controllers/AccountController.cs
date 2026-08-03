using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto_POO_IIPAC_2026.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    mensaje = "Debe ingresar correo y contraseña."
                });
            }


            var usuario =
                await _userManager.FindByEmailAsync(
                    request.Email);


            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos."
                });
            }


            var resultado =
                await _signInManager.PasswordSignInAsync(
                    usuario,
                    request.Password,
                    isPersistent: false,
                    lockoutOnFailure: false);


            if (!resultado.Succeeded)
            {
                return Unauthorized(new
                {
                    mensaje = "Correo o contraseña incorrectos."
                });
            }


            var roles =
                await _userManager.GetRolesAsync(usuario);


            return Ok(new
            {
                mensaje = "Inicio de sesión exitoso.",
                usuario = usuario.Email,
                roles = roles
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok(new
            {
                mensaje = "Sesión cerrada correctamente."
            });
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(new
                {
                    mensaje = "No hay una sesión iniciada."
                });
            }


            var email = User.Identity.Name;

            var usuario =
                await _userManager.FindByNameAsync(email!);


            if (usuario == null)
            {
                return Unauthorized();
            }


            var roles =
                await _userManager.GetRolesAsync(usuario);


            return Ok(new
            {
                usuario = usuario.Email,
                roles = roles
            });
        }

        [HttpGet("access-denied")]
        public IActionResult AccessDenied()
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new
                {
                    mensaje = "No tiene permisos para realizar esta acción."
                });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";
    }
}