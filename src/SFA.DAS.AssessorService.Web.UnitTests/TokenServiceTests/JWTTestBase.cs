using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Web.UnitTests.TokenServiceTests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Moq;
    using SFA.DAS.AssessorService.Web.Infrastructure;
    using SFA.DAS.AssessorService.Web.Services;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class JWTTestBase
    {
        protected const string TokenEncodingKey = "Wt+69DPlA9wjXl79V9N67bR4cpn9+3zZmgLJHBXy2aQ=";
        protected static Mock<ICache> Cache;
        protected static Mock<IHttpContextAccessor> ContextAccessor;
        protected static TokenService TokenService;

        public static void Setup()
        {
            SystemTime.UtcNow = () => new DateTime(2018, 02, 15, 13, 0, 0);

            Cache = new Mock<ICache>();

            ContextAccessor = new Mock<IHttpContextAccessor>();

            ContextAccessor
                .Setup(c => c.HttpContext)
                .Returns(new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim("http://schemas.portal.com/ukprn", "12345")
                    }))
                });
            
            TokenService = new TokenService(Cache.Object, new WebConfiguration()
            {
                Api = new ApiSettings()
                {
                    TokenEncodingKey = TokenEncodingKey
                }
            });
        }

        protected static string GenerateJwt(DateTime? expires = null)
        {
            var expireDateTime = expires ?? new DateTime(2018, 02, 15, 13, 30, 0);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenEncodingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("ukprn", "12345", ClaimValueTypes.String)
            };

            var token = new JwtSecurityToken(
                issuer: "sfa.das.assessorservice",
                audience: "sfa.das.assessorservice.api",
                claims: claims,
                expires: expireDateTime,
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
