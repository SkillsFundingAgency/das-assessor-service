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
            Cache.Setup(c => c.GetString("12345")).Returns(_jwt);
        };

        Because of = () =>
        {
            _result = TokenService.GetJwt("USERID");
            _token = new JwtSecurityTokenHandler().ReadToken(_result);
        };

        Machine.Specifications.It should_return_a_new_token = () =>
        {
            _token.ValidTo.Should().Be(new DateTime(2018, 02, 15, 14, 30, 0));
        };

        Machine.Specifications.It should_attempt_to_retrieve_the_jwt_from_the_cache = () =>
        {
            Cache.Verify(c => c.SetString("12345", Moq.It.IsAny<string>()));
        };

        Machine.Specifications.It should_not_be_the_original_jwt = () =>
        {
            _result.Should().NotBe(_jwt);
        };
    }
}
