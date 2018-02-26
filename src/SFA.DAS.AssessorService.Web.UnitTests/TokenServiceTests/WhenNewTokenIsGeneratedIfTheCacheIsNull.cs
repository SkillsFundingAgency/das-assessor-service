using System;
using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Web.UnitTests.TokenServiceTests
{
    [TestFixture]
    public class WhenNewTokenIsGeneratedIfTheCacheIsNull : JWTTestBase
    {
        private static string _result;
        private static string _jwt;
        private static SecurityToken _securityToken;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _jwt = GenerateJwt();
            Cache.Setup(c => c.GetString("userid1")).Returns(_jwt);
        }

        [Test]
        public void Should_return_a_jwt()
        {
            _result = TokenService.GetJwt("USERID");
            _securityToken = new JwtSecurityTokenHandler().ReadToken(_result);


            _result = TokenService.GetJwt("USERID");
            _securityToken.ValidTo.Should().Be(new DateTime(2018, 02, 15, 13, 30, 0));
        }
    }
}
