using System;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Web.UnitTests.TokenServiceTests
{
    //[TestFixture]
    //public class WhenAnExpiredTokenIsRegeneratedWithANewExpiryDate : JWTTestBase
    //{
    //    private static string _result;
    //    private static SecurityToken _token;
    //    private static string _jwt;

    //    [SetUp]
    //    public void Arrange()
    //    {
    //        Setup();

    //        _jwt = GenerateJwt(new DateTime(2018, 02, 15, 10, 00, 0));
    //        Cache.Setup(c => c.GetString("userid1")).Returns(_jwt);
    //    }

    //    [Test]
    //    public void Should_be_a_valid_token()
    //    {
    //        _result = TokenService.GetJwt("USERID");
    //        _token = new JwtSecurityTokenHandler().ReadToken(_result);
    //        _token.ValidTo.Should().Be(new DateTime(2018, 02, 15, 13, 30, 0));
    //    }

    //    [Test]
    //    public void Should_not_be_a_jwt()
    //    {
    //        _result = TokenService.GetJwt("USERID");
    //        _token = new JwtSecurityTokenHandler().ReadToken(_result);
    //        _result.Should().NotBe(_jwt);
    //    }
    //}
}
