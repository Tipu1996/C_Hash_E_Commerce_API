using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eCommerceAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace eCommerceAPI.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(Users user)
        {
            var issuer = _configuration["JwtOptions:Issuer"];
            var audience = _configuration["JwtOptions:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["JwtOptions:SigningKey"] ?? throw new InvalidOperationException("Jwt:Key is null in configuration."));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Log the generated token
            Console.WriteLine("Generated JWT Token:");
            Console.WriteLine(tokenHandler.WriteToken(token));

            return tokenHandler.WriteToken(token);
        }
    }

}