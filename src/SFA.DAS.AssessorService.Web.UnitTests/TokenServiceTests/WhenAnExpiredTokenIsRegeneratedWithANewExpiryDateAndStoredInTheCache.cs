namespace SFA.DAS.AssessorService.Application.MSpec.UnitTests
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using FluentAssertions;
    using Machine.Specifications;
    using Microsoft.IdentityModel.Tokens;

    using SFA.DAS.AssessorService.Web.Infrastructure;
    using SFA.DAS.AssessorService.Web.UnitTests.TokenServiceTests;

    [Subject("JWTTest")]
    public class WhenAnExpiredTokenIsRegeneratedWithANewExpiryDateAndStoredInTheCache : JWTTestBase
    {
        private static string _result;
        private static SecurityToken _token;
        private static string _jwt;

        Establish context = () =>
        {
            Setup();

            SystemTime.UtcNow = () => new DateTime(2018, 02, 15, 14, 0, 0);

            _jwt = GenerateJwt();
            Cache.Setup(c => c.GetString("userid1")).Returns(_jwt);
        };

        Because of = () =>
        {          
            _result = TokenService.GetJwt();
           _token = new JwtSecurityTokenHandler().ReadToken(_result);                        
        };

        Machine.Specifications.It should_be_a_valid_token = () =>
        {
            _token.ValidTo.Should().Be(new DateTime(2018, 02, 15, 14, 30, 0));         
        };

        Machine.Specifications.It should_call_set_string = () =>
        {            
            Cache.Verify(c => c.SetString("userid1", Moq.It.IsAny<string>()));
        };

        Machine.Specifications.It should_not_be_a_jwt = () =>
         {
             _result.Should().NotBe(_jwt);
         };
    }
}
