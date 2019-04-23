using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Login
{
    [TestFixture]
    public class When_User_has_no_roles : LoginHandlerTestsBase
    {
//        [Test]
//        public void Then_invalid_role_is_returned()
//        {
//            var response = Handler.Handle(new LoginRequest() {Roles = new List<string>()}, new CancellationToken()).Result;
//            response.Result.Should().Be(LoginResult.InvalidRole);
//        }
    }
}