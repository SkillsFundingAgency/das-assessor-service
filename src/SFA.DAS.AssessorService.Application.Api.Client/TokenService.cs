using System;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class TokenService : ITokenService
    {
        private readonly ICache _cache;
        private readonly IWebConfiguration _configuration;

        public TokenService(ICache cache, IWebConfiguration configuration)
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
                try
                {
                    new JwtBuilder()
                        .WithSecret(_configuration.Api.TokenEncodingKey)
                        .MustVerifySignature()
                        .Decode(result);
                }
                catch (TokenExpiredException expired)
                {
                    result = GetNewToken(userKey);
                    _cache.SetString(userKey, result);
                    return result;
                }
            }
            
            return result;
        }

        private string GetNewToken(string ukprn)
        {
            var token = new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_configuration.Api.TokenEncodingKey)
                .Issuer("sfa.das.assessorservice")
                .Audience("sfa.das.assessorservice.api")
                .ExpirationTime(DateTime.Now.AddMinutes(5))
                .AddClaim("ukprn", ukprn)
                .Build();

            return token;
            
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Api.TokenEncodingKey));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var claims = new[]
            //{
            //    new Claim("ukprn", ukprn, ClaimValueTypes.String)
            //};

            //var newToken = new JwtSecurityToken(
            //    issuer: "sfa.das.assessorservice",
            //    audience: "sfa.das.assessorservice.api",
            //    claims: claims,
            //    expires: SystemTime.UtcNow().AddMinutes(30),
            //    signingCredentials: creds);

            //return new JwtSecurityTokenHandler().WriteToken(newToken);
        }
    }
}