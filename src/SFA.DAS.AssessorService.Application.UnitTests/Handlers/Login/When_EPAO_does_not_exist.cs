﻿using System.Collections.Generic;
using System.Threading;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Login
{
//    [TestFixture]
//    public class When_EPAO_does_not_exist : LoginHandlerTestsBase
//    {
//        [Test]
//        public void Then_not_registered_is_returned()
//        {
//            OrgQueryRepository.Setup(r => r.GetByUkPrn(12345)).ReturnsAsync(default(Organisation));
//
//            var response = Handler.Handle(new LoginRequest() { Roles = new List<string>() { "ABC", "DEF", "EPA" }, UkPrn = 12345}, new CancellationToken()).Result;
//            response.Result.Should().Be(LoginResult.NotRegistered);
//        }
//    }
}