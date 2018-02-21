using System;

namespace SFA.DAS.AssessorService.Application.MSpec.UnitTests
{
    using FluentAssertions;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Web.Infrastructure;
    using SFA.DAS.AssessorService.Web.UnitTests.TokenServiceTests;

    [Subject("JWTTest")]
    // WhenANewTokenIsCreatedAndIsNotInCache
    public class WhenANewTokenIsStoredInTheCacheIfTheCacheIsNull : JWTTestBase
    {
        private static string _result;
        private static string _jwt;
        private static string _expectedToken;        

        Establish context = () =>
        {
            Setup();

            _jwt = GenerateJwt();
            Cache.Setup(c => c.GetString("USERID")).Returns(default(string));
        };

        Because of = () =>
        {           
            _result = TokenService.GetJwt("USERID");
            _expectedToken = GenerateJwt(DateService.GetNow().DateTime.AddMinutes(30));
        };
      
        Machine.Specifications.It should_call_set_string = () =>
        {
            Cache.Verify(c => c.SetString("USERID", Moq.It.IsAny<string>()));
        };

        Machine.Specifications.It should_return_expected_token = () =>
        {
            _result.Should().Be(_expectedToken);
        };
    }
}
