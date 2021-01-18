using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers;
using SFA.DAS.AssessorService.Domain.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.LearnerDetails.GetEarliestWithdrawalDateTests
{
    [TestFixture]
    public class When_called_for_standard
    {
        private GetEarliestWithdrawalDateHandler _sut;
        private Mock<IDateTimeHelper> _mockDateTimeHelper;

        [SetUp]
        public void Arrange()
        {
            _mockDateTimeHelper = new Mock<IDateTimeHelper>();

            _sut = new GetEarliestWithdrawalDateHandler(_mockDateTimeHelper.Object); 
        }

        [TestCase(2020, 12, 01, 100)]
        [TestCase(2019, 12, 01, 200)]
        [TestCase(2018, 12, 01, 300)]
        [TestCase(2017, 12, 01, 400)]
        [TestCase(2016, 12, 01, 87)]
        public async Task Then_date_plus_six_months_is_returned(int year, int month, int day, int standardCode)
        {
            // Arrange
            DateTime currentDate = new DateTime(year, month, day);
            _mockDateTimeHelper
                .SetupGet(r => r.DateTimeNow)
                .Returns(currentDate);

            // Act
            var result = await _sut.Handle(new GetEarliestWithdrawalDateRequest(Guid.NewGuid(), standardCode), new CancellationToken());

            // Assert
            result.Should().Be(currentDate.AddMonths(6));
        }
    }
}