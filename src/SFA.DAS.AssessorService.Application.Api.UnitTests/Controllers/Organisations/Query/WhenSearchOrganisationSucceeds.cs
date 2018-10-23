﻿using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Query
{
    public class WhenSearchOrganisationSucceeds : OrganisationQueryBase
    {
        private Organisation _organisation;
        private IActionResult _result;
      
        [SetUp]
        public void Arrange()
        {
            Setup();

            _organisation = Builder<Organisation>.CreateNew().Build();

            OrganisationQueryRepositoryMock.Setup(q => q.GetByUkPrn(Moq.It.IsAny<long>()))
                .Returns(Task.FromResult((_organisation)));

            _result = OrganisationQueryController.SearchOrganisation(10000000).Result;
        }

        [Test]
        public void ThenTheResultReturnsOkStatus()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public void ThenTheQueryReturnsTheCorrectResult()
        {
            if(!(_result is OkObjectResult result))
                ((OkObjectResult) null).Should().NotBeNull();
            else 
                result.Value.Should().BeOfType<OrganisationResponse>();

        }  
    }
}