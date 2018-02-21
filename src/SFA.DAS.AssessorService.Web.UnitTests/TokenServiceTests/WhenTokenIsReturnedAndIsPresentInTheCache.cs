namespace SFA.DAS.AssessorService.Application.MSpec.UnitTests
{
    using FluentAssertions;
    using Machine.Specifications;
    using SFA.DAS.AssessorService.Web.UnitTests.TokenServiceTests;

    [Subject("JWTTest")]
    // WhenANewTokenIsCreatedAndIsInCache
    public class WhenTokenIsReturnedAndIsPresentInTheCache : JWTTestBase
    {
        private static string _result;
        private static string _jwt;

        Establish context = () =>
        {
            Setup();

            _jwt = GenerateJwt();
            Cache.Setup(c => c.GetString("USERID")).Returns(_jwt);
        };

        Because of = () =>
        {
            _result = TokenService.GetJwt("USERID");
        };

        Machine.Specifications.It should_return_a_jwt = () =>
        {
            _result.Should().Be(_jwt);
        };
    }
}
