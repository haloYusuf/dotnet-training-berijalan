using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IDMS.Infrastructure.Data;
using IDMS.Modules.Api.Master.Dto.Request.Auth;
using IDMS.Modules.Api.Master.Dto.Response;
using IDMS.Shared.Domain.Entities;
using IDMS.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IDMS.Modules.Api.Master.Services.Impl
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

        public async Task<ResLoginDto> loginAsync(ReqLoginDto req)
        {
            var user = await _db.MstUsers.FirstOrDefaultAsync(e => e.Email == req.Email)
            ?? throw new BadRequestException("Invalid email or password");

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
                throw new BadRequestException("Invalid email or password");

            return new ResLoginDto
            {
                Email = user.Email,
                Token = GenerateToken(user)
            };
            // throw new NotImplementedException();
        }

        private string GenerateToken(MstUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> registerAsync(ReqMstUserCreateDto req)
        {
            if (await _db.MstUsers.AnyAsync(e => e.Email == req.Email && e.IsActive))
            {
                throw new BadRequestException("Email already registered");
            }

            var user = new MstUser
            {
                Email = req.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
                FullName = req.FullName,
                IsActive = true,
            };

            _db.MstUsers.Add(user);
            return await _db.SaveChangesAsync() > 0;
        }
    }
}