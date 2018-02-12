using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.AssessorService.Web.Services;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public class TokenService : ITokenService
    {
        private readonly ICache _cache;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;

        public TokenService(ICache cache, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _cache = cache;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }

        public string GetJwt()
        {
            var userId = _contextAccessor.HttpContext.User
                .FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            var result = _cache.GetString(userId);

            if (result == null)
            {
                result = GetNewToken();
                _cache.SetString(userId, result);
            }
            else
            {
                var token = new JwtSecurityTokenHandler().ReadToken(result);

                if (token.ValidTo >= SystemTime.UtcNow()) return result;

                result = GetNewToken();
                _cache.SetString(userId, result);
            }
            
            return result;
        }

        private string GetNewToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthOptions:TokenEncodingKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var newToken = new JwtSecurityToken(
                issuer: "sfa.das.assessorservice",
                audience: "sfa.das.assessorservice.api",
                expires: SystemTime.UtcNow().AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(newToken);
        }
    }
}