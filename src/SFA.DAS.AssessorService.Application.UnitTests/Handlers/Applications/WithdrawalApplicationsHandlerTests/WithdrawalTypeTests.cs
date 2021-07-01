using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Applications.WithdrawalApplicationsHandlerTests
{
    public class WithdrawalTypeTests : WithdrawalApplicationHandlerTestsBase
    {
        [Test]
        public async Task When_Versions_Is_Null_Then_WithdrawalType_Should_Be_Register()
        {
            // Arrange

            ApplyRepository.Setup(m => m.GetWithdrawalApplications(It.IsAny<string>(), "New", "SubmittedDate", 0, 10, 1))
                .ReturnsAsync(new ApplicationsResult()
                {
                    TotalCount = 1,
                    PageOfResults = new List<Domain.Entities.ApplicationListItem>()
                    {
                            new Domain.Entities.ApplicationListItem()
                            {

                                Versions = null  
                            }
                    }
                });

            // Act

            var paginatedList = await Handler.Handle(
                new WithdrawalApplicationsRequest(
                    new Guid().ToString(), // organisation id
                    "New", // reviewstatus
                    "SubmittedDate", // sortcolumn
                    0, // sort ascending
                    10, // page size
                    1, // page index
                    6 // page set size                
                ), 
                new CancellationToken());

            // Assert

            paginatedList.Should().NotBeNull();
            paginatedList.Items.Should().HaveCount(1);
            paginatedList.Items[0].WithdrawalType.Should().Be(WithdrawalTypes.Register);
        }

        [Test]
        public async Task When_Versions_Is_Empty_Then_WithdrawalType_Should_Be_Register()
        {
            // Arrange

            ApplyRepository.Setup(m => m.GetWithdrawalApplications(It.IsAny<string>(), "New", "SubmittedDate", 0, 10, 1))
                .ReturnsAsync(new ApplicationsResult()
                {
                    TotalCount = 1,
                    PageOfResults = new List<Domain.Entities.ApplicationListItem>()
                    {
                            new Domain.Entities.ApplicationListItem()
                            {

                                Versions = "[]"
                            }
                    }
                });

            // Act

            var paginatedList = await Handler.Handle(
                new WithdrawalApplicationsRequest(
                    new Guid().ToString(), // organisation id
                    "New", // reviewstatus
                    "SubmittedDate", // sortcolumn
                    0, // sort ascending
                    10, // page size
                    1, // page index
                    6 // page set size                
                ),
                new CancellationToken());

            // Assert

            paginatedList.Should().NotBeNull();
            paginatedList.Items.Should().HaveCount(1);
            paginatedList.Items[0].WithdrawalType.Should().Be(WithdrawalTypes.Register);
        }

        [Test]
        public async Task When_Versions_Has_One_Version_Then_WithdrawalType_Should_Be_Version()
        {
            // Arrange

            ApplyRepository.Setup(m => m.GetWithdrawalApplications(It.IsAny<string>(), "New", "SubmittedDate", 0, 10, 1))
                .ReturnsAsync(new ApplicationsResult()
                {
                    TotalCount = 1,
                    PageOfResults = new List<Domain.Entities.ApplicationListItem>()
                    {
                            new Domain.Entities.ApplicationListItem()
                            {

                                Versions = "[\"1.0\"]" 
                            }
                    }
                });

            // Act

            var paginatedList = await Handler.Handle(
                new WithdrawalApplicationsRequest(
                    new Guid().ToString(), // organisation id
                    "New", // reviewstatus
                    "SubmittedDate", // sortcolumn
                    0, // sort ascending
                    10, // page size
                    1, // page index
                    6 // page set size                
                ),
                new CancellationToken());

            // Assert

            paginatedList.Should().NotBeNull();
            paginatedList.Items.Should().HaveCount(1);
            paginatedList.Items[0].WithdrawalType.Should().Be(WithdrawalTypes.Version);
        }

        [Test]
        public async Task When_Versions_Has_All_Versions_Then_WithdrawalType_Should_Be_Standard()
        {
            // Arrange

            ApplyRepository.Setup(m => m.GetWithdrawalApplications(It.IsAny<string>(), "New", "SubmittedDate", 0, 10, 1))
                .ReturnsAsync(new ApplicationsResult()
                {
                    TotalCount = 1,
                    PageOfResults = new List<Domain.Entities.ApplicationListItem>()
                    {
                            new Domain.Entities.ApplicationListItem()
                            {

                                Versions = "[\"1.0\",\"1.1\",\"1.2\",\"1.3\"]"
                            }
                    }
                });

            // Act

            var paginatedList = await Handler.Handle(
                new WithdrawalApplicationsRequest(
                    new Guid().ToString(), // organisation id
                    "New", // reviewstatus
                    "SubmittedDate", // sortcolumn
                    0, // sort ascending
                    10, // page size
                    1, // page index
                    6 // page set size                
                ),
                new CancellationToken());

            // Assert

            paginatedList.Should().NotBeNull();
            paginatedList.Items.Should().HaveCount(1);
            paginatedList.Items[0].WithdrawalType.Should().Be(WithdrawalTypes.Standard);
        }
    }
}
