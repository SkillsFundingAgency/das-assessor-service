﻿using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Contacts.Query
{
    public class WhenSearchByUserNameSucceeds : ContactsQueryBase
    {
        private static Contact _contactResponse;
        private IActionResult _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            MappingBootstrapper.Initialize();

            _contactResponse = Builder<Contact>.CreateNew().Build();

            ContactQueryRepositoryMock.Setup(q => q.GetContact(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((_contactResponse)));

            _result = ContactQueryController.SearchContactByUserName("TestUser").Result;
        }

        [Test]
        public void ThenTheResultReturnsOkStatus()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public void ThenTheResultReturnsValidData()
        {
            var result = _result as OkObjectResult;
            (result.Value is ContactResponse).Should().BeTrue();
        }
    }
}
