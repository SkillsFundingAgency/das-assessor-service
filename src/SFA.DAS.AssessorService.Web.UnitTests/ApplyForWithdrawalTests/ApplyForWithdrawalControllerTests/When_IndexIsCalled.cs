﻿using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Web.UnitTests.ApplyForWithdrawalTests.ApplyForWithdrawalControllerTests;

public class When_IndexIsCalled : ApplyForWithdrawalControllerTestsBase
{
    [Test]
    public async Task RedirectsToDashboard()
    {
        var result = await  _sut.Index();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<RedirectToActionResult>());
            Assert.That(result.As<RedirectToActionResult>().ControllerName, Is.EqualTo("Dashboard"));
            Assert.That(result.As<RedirectToActionResult>().ActionName, Is.EqualTo("Index"));
        });
    }
}