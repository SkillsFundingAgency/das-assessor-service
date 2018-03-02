using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Web.UnitTests.TokenServiceTests
{
    //[TestFixture]
    //public class WhenANewTokenIsStoredInTheCacheIfTheCacheIsNull : JWTTestBase
    //{
    //    private static string _result;
    //    private static string _jwt;
    //    private static string _expectedToken;        

    //    [SetUp]
    //    public void Arrange()
    //    {
    //        Setup();

    //        _jwt = GenerateJwt();
    //        Cache.Setup(c => c.GetString("USERID")).Returns(default(string));
    //    }
      
    //    [Test]
    //    public void Should_call_set_string()
    //    {
    //        _result = TokenService.GetJwt("USERID");
    //        _expectedToken = GenerateJwt(DateService.GetNow().DateTime.AddMinutes(30));
    //        Cache.Verify(c => c.SetString("USERID", Moq.It.IsAny<string>()));
    //    }

    //    [Test]
    //    public void Should_return_expected_token()
    //    {
    //        _result = TokenService.GetJwt("USERID");
    //        _expectedToken = GenerateJwt(DateService.GetNow().DateTime.AddMinutes(30));
    //        _result.Should().Be(_expectedToken);
    //    }
    //}
}