using Microsoft.AspNetCore.Mvc;
using AzureDevNexus.Server.Services;
using AzureDevNexus.Shared.Models;

namespace AzureDevNexus.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { Message = "Username and password are required" });
                }

                var response = await _authService.AuthenticateAsync(request);

                if (response == null)
                {
                    return Unauthorized(new { Message = "Invalid username or password" });
                }

                _logger.LogInformation("User {Username} logged in successfully", request.Username);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", request.Username);
                return StatusCode(500, new { Message = "An error occurred during authentication" });
            }
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            // In a real application, you might want to blacklist the token
            // For now, we'll just return a success response
            return Ok(new { Message = "Logged out successfully" });
        }

        [HttpGet("profile")]
        public async Task<ActionResult<User>> GetProfile([FromHeader(Name = "Authorization")] string? authorization)
        {
            try
            {
                if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
                {
                    return Unauthorized(new { Message = "Authorization header is required" });
                }

                var token = authorization.Substring("Bearer ".Length);
                var jwtService = HttpContext.RequestServices.GetRequiredService<IJwtService>();
                var userId = jwtService.GetUserIdFromToken(token);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "Invalid or expired token" });
                }

                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, new { Message = "An error occurred while retrieving profile" });
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromHeader(Name = "Authorization")] string? authorization)
        {
            try
            {
                if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith("Bearer "))
                {
                    return Unauthorized(new { Message = "Authorization header is required" });
                }

                var token = authorization.Substring("Bearer ".Length);
                var jwtService = HttpContext.RequestServices.GetRequiredService<IJwtService>();
                var userId = jwtService.GetUserIdFromToken(token);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Message = "Invalid or expired token" });
                }

                var user = await _authService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }

                var newToken = jwtService.GenerateToken(user);

                var response = new RefreshTokenResponse
                {
                    Token = newToken,
                    RefreshToken = Guid.NewGuid().ToString(), // Simple refresh token for development
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return StatusCode(500, new { Message = "An error occurred while refreshing token" });
            }
        }
    }
}
