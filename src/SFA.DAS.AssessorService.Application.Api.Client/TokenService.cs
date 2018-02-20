using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class TokenService : ITokenService
    {
        private readonly ICache _cache;
        private readonly IConfiguration _configuration;

        public TokenService(ICache cache, IConfiguration configuration)
        {
            _cache = cache;
            _configuration = configuration;
        }

        public string GetJwt(string userKey)
        {
            //var ukprn = _contextAccessor.HttpContext.User
            //    .FindFirst("http://schemas.portal.com/ukprn").Value;

            var result = _cache.GetString(userKey);

            if (result == null)
            {
                result = GetNewToken(userKey);
                _cache.SetString(userKey, result);
            }
            else
            {
                var token = new JwtSecurityTokenHandler().ReadToken(result);

                if (token.ValidTo >= SystemTime.UtcNow()) return result;

                result = GetNewToken(userKey);
                _cache.SetString(userKey, result);
            }
            
            return result;
        }

        private string GetNewToken(string ukprn)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthOptions:TokenEncodingKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("ukprn", ukprn, ClaimValueTypes.String)
            };

            var newToken = new JwtSecurityToken(
                issuer: "sfa.das.assessorservice",
                audience: "sfa.das.assessorservice.api",
                claims: claims,
                expires: SystemTime.UtcNow().AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(newToken);
        }
    }
}