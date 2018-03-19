using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.UnitTests.Login
{
    [TestFixture]
    public class When_User_does_not_have_correct_role :LoginHandlerTestsBase
    {
        [Test]
        public void Then_invalid_role_is_returned()
        {
            var response = Handler.Handle(new LoginRequest() {Roles = new List<string>() {"ABC", "DEF"}}, new CancellationToken()).Result;
            response.Result.Should().Be(LoginResult.InvalidRole);
        }
    }
}