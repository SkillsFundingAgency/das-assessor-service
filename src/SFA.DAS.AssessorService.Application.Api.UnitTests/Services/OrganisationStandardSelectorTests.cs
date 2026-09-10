using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services
{
    [TestFixture]
    public class OrganisationStandardSelectorTests
    {
        private const string OrganisationId = "EPA0001";
        private const string StandardReference = "ST0490";
        private const int LarsCode = 12345;

        private Mock<ILogger<OrganisationStandardSelector>> _loggerMock;
        private OrganisationStandardSelector _selector;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger<OrganisationStandardSelector>>();
            _selector = new OrganisationStandardSelector(_loggerMock.Object);
        }

        private static OrganisationStandard CreateRow(int id, int standardCode, System.DateTime? effectiveTo = null)
        {
            return new OrganisationStandard
            {
                Id = id,
                EndPointAssessorOrganisationId = OrganisationId,
                StandardReference = StandardReference,
                StandardCode = standardCode,
                EffectiveTo = effectiveTo
            };
        }

        [Test]
        public void Select_ShouldReturnNotFound_WhenNoRowsExist()
        {
            var result = _selector.Select(new List<OrganisationStandard>(), LarsCode);

            result.Result.Should().Be(OrganisationStandardSelectionResult.NotFound);
            result.OrganisationStandard.Should().BeNull();
            result.TotalRowCount.Should().Be(0);
        }

        [Test]
        public void Select_ShouldReturnTheOnlyRow_WhenExactlyOneRowExists_RegardlessOfLarsCode()
        {
            var row = CreateRow(id: 101, standardCode: 999); 

            var result = _selector.Select(new List<OrganisationStandard> { row }, LarsCode);

            result.Result.Should().Be(OrganisationStandardSelectionResult.Found);
            result.OrganisationStandard.Should().Be(row);
            result.TotalRowCount.Should().Be(1);
        }

        [Test]
        public void Select_ShouldReturnTheMatchingRow_WhenMultipleRowsExist_AndExactlyOneMatchesLarsCode()
        {
            var matchingRow = CreateRow(id: 101, standardCode: LarsCode);
            var otherRow = CreateRow(id: 102, standardCode: 999);

            var result = _selector.Select(new List<OrganisationStandard> { otherRow, matchingRow }, LarsCode);

            result.Result.Should().Be(OrganisationStandardSelectionResult.Found);
            result.OrganisationStandard.Should().Be(matchingRow);
            result.TotalRowCount.Should().Be(2);
            result.MatchingLarsCodeCount.Should().Be(1);
        }

        [Test]
        public void Select_ShouldReturnAmbiguous_WhenMultipleRowsExist_AndNoneMatchLarsCode()
        {
            var rowA = CreateRow(id: 101, standardCode: 111);
            var rowB = CreateRow(id: 102, standardCode: 222);

            var result = _selector.Select(new List<OrganisationStandard> { rowA, rowB }, LarsCode);

            result.Result.Should().Be(OrganisationStandardSelectionResult.Ambiguous);
            result.OrganisationStandard.Should().BeNull();
            result.TotalRowCount.Should().Be(2);
            result.MatchingLarsCodeCount.Should().Be(0);
        }

        [Test]
        public void Select_ShouldReturnTheActiveRow_WhenMultipleRowsMatchTheSameLarsCode_AndOnlyOneIsActive()
        {
            var withdrawnRow = CreateRow(id: 101, standardCode: LarsCode, effectiveTo: System.DateTime.Today.AddDays(-1));
            var activeRow = CreateRow(id: 102, standardCode: LarsCode, effectiveTo: null);

            var result = _selector.Select(new List<OrganisationStandard> { withdrawnRow, activeRow }, LarsCode);

            result.Result.Should().Be(OrganisationStandardSelectionResult.Found);
            result.OrganisationStandard.Should().Be(activeRow);
            result.MatchingLarsCodeCount.Should().Be(2);
        }

        [Test]
        public void Select_ShouldReturnAmbiguous_WhenMultipleRowsMatchTheSameLarsCode_AndNoneOrMoreThanOneIsActive()
        {
            var rowA = CreateRow(id: 101, standardCode: LarsCode, effectiveTo: null);
            var rowB = CreateRow(id: 102, standardCode: LarsCode, effectiveTo: null);

            var result = _selector.Select(new List<OrganisationStandard> { rowA, rowB }, LarsCode);

            result.Result.Should().Be(OrganisationStandardSelectionResult.Ambiguous);
            result.OrganisationStandard.Should().BeNull();
            result.MatchingLarsCodeCount.Should().Be(2);
        }
    }
}
