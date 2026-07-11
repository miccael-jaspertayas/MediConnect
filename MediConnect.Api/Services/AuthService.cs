using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediConnect.Api.Data;
using MediConnect.Api.Dtos;
using MediConnect.Api.Models;

namespace MediConnect.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var emailTaken = await _db.Users.AnyAsync(u => u.Email == request.Email);
            if (emailTaken) return false;

            var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User(request.Email, hash);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // Create a blank patient profile right away so the mobile app will not show "no profile yet"
            var patient = new Patient(user.UserID);
            _db.Patients.Add(patient);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _db.Users
                .Include(u => u.Patient)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user is null) return null;
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return null;

            var token = GenerateJwt(user);

            return new LoginResponse
            {
                Token = token,
                UserID = user.UserID,
                PatientID = user.Patient!.PatientID
            };
        }

        private string GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserID", user.UserID.ToString()),
                new Claim("PatientID", user.Patient!.PatientID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var expiresMinutes = int.Parse(_config["Jwt:ExpiresMinutes"] ?? "120");

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
