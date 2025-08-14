using AzureDevNexus.Server.Data;
using AzureDevNexus.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureDevNexus.Server.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request);
        Task<User?> GetUserByIdAsync(string userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> ValidateCredentialsAsync(string username, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly AzureDevNexusContext _context;
        private readonly IJwtService _jwtService;

        public AuthService(AzureDevNexusContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return null;
            }

            // In a real application, you would hash and verify the password
            // For development purposes, we'll use a simple comparison
            if (!await ValidateCredentialsAsync(request.Username, request.Password))
            {
                return null;
            }

            var token = _jwtService.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                RefreshToken = Guid.NewGuid().ToString(), // Simple refresh token for development
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = user
            };
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            // In a real application, you would implement proper password hashing and verification
            // For development purposes, we'll use a simple check against seeded data
            
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return false;
            }

            // For development: check if password matches the seeded data
            // In production, use proper password hashing (e.g., BCrypt, Argon2)
            return password == "password123"; // This should be replaced with proper password validation
        }
    }
}
