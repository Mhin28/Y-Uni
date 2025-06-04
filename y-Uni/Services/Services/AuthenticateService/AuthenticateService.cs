using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repositories.ViewModels.AutheticateModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.AuthenticateService
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private readonly IConfiguration _config;
        public AuthenticateService(IConfiguration config)
        {
            _config = config;
            _jwtHandler = new JwtSecurityTokenHandler();
        }
        public string decodeToken(string jwtToken, string nameClaim)
        {
            Claim? claim = _jwtHandler.ReadJwtToken(jwtToken).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));
            return claim != null ? claim.Value : "Error!!!";
        }

        public string GenerateJWT(LoginResModel User)
        {
            // Retrieve JWT Key
            string? jwtKey = _config["JwtSettings:JwtKey"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is missing in configuration.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userRoles = new List<string> { "User" };

            var claims = new List<Claim>
            {
                new Claim("userid", User.UserId.ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(6),
                signingCredentials: credential
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
