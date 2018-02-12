using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.Services;

namespace SFA.DAS.AssessorService.Web.UnitTests.TokenServiceTests
{
    [TestFixture]
    public class WhenGetJwtIsCalled
    {
        private const string tokenEncodingKey = "Wt+69DPlA9wjXl79V9N67bR4cpn9+3zZmgLJHBXy2aQ=";
        private Mock<ICache> _cache;
        private Mock<IHttpContextAccessor> _contextAccessor;
        private TokenService _tokenService;
        
        private string GenerateJwt(DateTime? expires = null)
        {
            var expireDateTime = expires ?? new DateTime(2018, 02, 15, 13, 30, 0);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenEncodingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "sfa.das.assessorservice",
                audience: "sfa.das.assessorservice.api",
                expires: expireDateTime,
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        [SetUp]
        public void Arrange()
        {
            SystemTime.UtcNow = () => new DateTime(2018, 02, 15, 13, 0, 0);

            _cache = new Mock<ICache>();

            _contextAccessor = new Mock<IHttpContextAccessor>();

            _contextAccessor
                .Setup(c => c.HttpContext)
                .Returns(new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", "userid1")
                    }))
                });

            var configuration = new Mock<IConfiguration>();
            configuration.Setup(c => c["AuthOptions:TokenEncodingKey"]).Returns(tokenEncodingKey);

            _tokenService = new TokenService(_cache.Object, _contextAccessor.Object, configuration.Object);
        }

        [Test]
        public void ThenTheTokenIsReturnedIfPresentInTheCache()
        {
            
            var jwt = GenerateJwt();
            _cache.Setup(c => c.GetString("userid1")).Returns(jwt);

            var result = _tokenService.GetJwt();

            result.Should().Be(jwt);
        }

        [Test]
        public void ThenAnExpiredTokenIsRegeneratedWithANewExpiryDate()
        {
            SystemTime.UtcNow = () => new DateTime(2018, 02, 15, 14, 0, 0);
            var jwt = GenerateJwt();
            _cache.Setup(c => c.GetString("userid1")).Returns(jwt); 

            var result = _tokenService.GetJwt();
            
            var token = new JwtSecurityTokenHandler().ReadToken(result);

            token.ValidTo.Should().Be(new DateTime(2018, 02, 15, 14, 30, 0));
            result.Should().NotBe(jwt);
        }

        [Test]
        public void ThenAnExpiredTokenIsRegeneratedWithANewExpiryDateAndStoredInTheCache()
        {
            SystemTime.UtcNow = () => new DateTime(2018, 02, 15, 14, 0, 0);
            var jwt = GenerateJwt();
            _cache.Setup(c => c.GetString("userid1")).Returns(jwt);

            var result = _tokenService.GetJwt();

            var token = new JwtSecurityTokenHandler().ReadToken(result);

            token.ValidTo.Should().Be(new DateTime(2018, 02, 15, 14, 30, 0));
            result.Should().NotBe(jwt);

            _cache.Verify(c => c.SetString("userid1", It.IsAny<string>()));
        }

        [Test]
        public void ThenANewTokenIsGeneratedIfTheCacheIsNull()
        {
            _cache.Setup(c => c.GetString("userid1")).Returns(default(string));

            var result = _tokenService.GetJwt();

            var token = new JwtSecurityTokenHandler().ReadToken(result);

            token.ValidTo.Should().Be(new DateTime(2018, 02, 15, 13, 30, 0));
        }

        [Test]
        public void ThenANewTokenIsStoredInTheCacheIfTheCacheIsNull()
        {
            _cache.Setup(c => c.GetString("userid1")).Returns(default(string));

            var result = _tokenService.GetJwt();

            _cache.Verify(c => c.SetString("userid1", It.IsAny<string>()));
            
            var expectedToken = GenerateJwt(SystemTime.UtcNow().AddMinutes(30));
            
            result.Should().Be(expectedToken);
        }
    }
}