using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Login
{
    [TestFixture]
    public class When_EPAO_is_Deleted : LoginHandlerTestsBase
    {
        //        [Test]
        //        public void Then_not_registered_is_returned()
        //        {
        //            OrgQueryRepository.Setup(r => r.GetByUkPrn(12345)).ReturnsAsync(new Organisation{Status = OrganisationStatus.Deleted});
        //
        //            var response = Handler.Handle(new LoginRequest() { Roles = new List<string>() { "ABC", "DEF", "EPA" }, UkPrn = 12345 }, new CancellationToken()).Result;
        //            response.Result.Should().Be(LoginResult.NotRegistered);
        //        }
    }
}