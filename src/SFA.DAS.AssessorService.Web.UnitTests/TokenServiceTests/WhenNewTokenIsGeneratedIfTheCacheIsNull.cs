namespace SFA.DAS.AssessorService.Application.MSpec.UnitTests
{
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.IdentityModel.Tokens;
    using SFA.DAS.AssessorService.Web.UnitTests.TokenServiceTests;
    using System;
    using System.IdentityModel.Tokens.Jwt;

    [Subject("JWTTest")]
    public class WhenNewTokenIsGeneratedIfTheCacheIsNull : JWTTestBase
    {
        private static string _result;
        private static string _jwt;
        private static SecurityToken _securityToken;

        Establish context = () =>
        {
            Setup();

            _jwt = GenerateJwt();
            Cache.Setup(c => c.GetString("userid1")).Returns(_jwt);
        };

        Because of = () =>
        {          
            _result = TokenService.GetJwt();
            _securityToken = new JwtSecurityTokenHandler().ReadToken(_result);

          
            _result = TokenService.GetJwt();
        };

        Machine.Specifications.It should_return_a_jwt = () =>
        {
            _securityToken.ValidTo.Should().Be(new DateTime(2018, 02, 15, 13, 30, 0));
        };
    }
}
