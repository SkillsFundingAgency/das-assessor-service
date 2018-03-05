using System;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using FluentAssertions;
using Machine.Specifications;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Exceptions;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.WebAPI.Organisations.Queries
{
    [Subject("AssessorService")]
    public class WhenSearchOrganisationIsPassedInvalidUkprn : OrganisationQueryBase
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
                var result = OrganisationQueryController.SearchOrganisation(10).Result;
            }
            catch (Exception exception)
            {
                _result = exception;
            }
        }

        [Test]
        public void ThenTheResultReturnBadRequestStatus()
        {
            _result.InnerException.Should().BeOfType<BadRequestException>();
        }
    }
}
