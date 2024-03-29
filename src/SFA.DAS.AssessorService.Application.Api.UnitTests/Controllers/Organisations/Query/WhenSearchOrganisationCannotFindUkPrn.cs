﻿using System;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Organisations.Query
{
    public class WhenSearchOrganisationCannotFindUkPrn : OrganisationQueryBase
    {
        private static Organisation _organisation;
        private Exception _result;

        [SetUp]
        public void Arrange()
        {
            Setup();

            _organisation = Builder<Organisation>.CreateNew().Build();

            OrganisationQueryRepositoryMock.Setup(q => q.GetByUkPrn(Moq.It.IsAny<int>()))
                .Returns(Task.FromResult<Organisation>(null));

            try
            {
                var result = OrganisationQueryController.SearchOrganisation(10000000).Result;
            }
            catch (Exception exception)
            {
                _result = exception;
            }           
        }

        [Test]
        public void ThenTheResultReturnsNotFoundStatus()
        {
            var result = _result.InnerException as ResourceNotFoundException;
            result.Should().NotBeNull();
        }
    }
}
