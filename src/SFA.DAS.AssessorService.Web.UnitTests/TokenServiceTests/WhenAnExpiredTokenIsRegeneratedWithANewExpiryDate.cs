namespace SFA.DAS.AssessorService.Application.MSpec.UnitTests
{
    using FluentAssertions;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Web.UnitTests.TokenServiceTests;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.IdentityModel.Tokens;
    using SFA.DAS.AssessorService.Web.Infrastructure;
    using System;

    [Subject("JWTTest")]
    public class WhenAnExpiredTokenIsRegeneratedWithANewExpiryDate : JWTTestBase
    {
        private static string _result;
        private static SecurityToken _token;
        private static string _jwt;

        Establish context = () =>
        {
            Setup();
            
            _jwt = GenerateJwt(new DateTime(2018, 02, 15, 10, 00, 0));
            Cache.Setup(c => c.GetString("userid1")).Returns(_jwt);
        };

        Because of = () =>
        {
            _result = TokenService.GetJwt("USERID");
            _token = new JwtSecurityTokenHandler().ReadToken(_result);        
        };

        Machine.Specifications.It should_be_a_valid_token = () =>
        {
            _token.ValidTo.Should().Be(new DateTime(2018, 02, 15, 13, 30, 0));
        };

        Machine.Specifications.It should_not_be_a_jwt = () =>
         {
             _result.Should().NotBe(_jwt);
         };
    }
}
