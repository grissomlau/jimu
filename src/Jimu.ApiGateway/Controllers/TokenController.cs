using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Jimu.ApiGateway.Controllers
{
    public class TokenRequest
    {
        [Required]
        public string Username { set; get; }

        [Required]
        public string Password { set; get; }
    }

    [Route("api/token")]
    public class TokenApiController : Controller
    {
        private readonly AuthService _authService;

        public TokenApiController(AuthService authService)
        {
            // AuthService is your own class that handles your application's authentication functions.
            // AuthService is injected via Controller's constructor and registered At startup.cs
            // Read more: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TokenRequest model)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest("Username and password must not be empty!");
            }

            // Authenticates username and password to your SQL Server database, for example.
            // If authentication is successful, return a user's claims.
            var claims = await _authService.TryLogin(model.Username, model.Password);
            if (claims == null)
            {
                return BadRequest("Invalid username or password!");
            }

            // As an example, AuthService.CreateToken can return Jose.JWT.Encode(claims, YourTokenSecretKey, Jose.JwsAlgorithm.HS256);
            var token = _authService.CreateToken(claims);
            return Ok(token);
        }
    }
}