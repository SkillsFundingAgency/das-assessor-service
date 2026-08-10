using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Data.UnitTests.Certificates
{
    public class WhenSystemGetsStandardMasks
    {
        private CertificateRepository _sut;
        private Mock<IAssessorUnitOfWork> _mockUnitOfWork;
        private DynamicParameters _capturedParameters;

        [SetUp]
        public void Arrange()
        {
            _mockUnitOfWork = new Mock<IAssessorUnitOfWork>();
            _sut = new CertificateRepository(_mockUnitOfWork.Object);

            _mockUnitOfWork
                .Setup(unitOfWork =>
                    unitOfWork.QueryStoredProcedureAsync<CertificateMask>(
                        "Certificates_GetStandardMasks",
                        It.IsAny<DynamicParameters>(),
                        It.IsAny<int?>(),
                        It.IsAny<CancellationToken>()))
                .Callback<string, DynamicParameters, int?, CancellationToken>(
                    (_, parameters, _, _) =>
                        _capturedParameters = parameters)
                .ReturnsAsync(new List<CertificateMask>
                {
                    new CertificateMask
                    {
                        CertificateType = "Standard",
                        CourseCode = "100",
                        CourseName = "Std Test",
                        CourseLevel = "3",
                        ProviderName = "Prov"
                    }
                });
        }

        [Test]
        public async Task Then_Repository_Queries_StoredProcedure_And_Returns_Masks()
        {
            var result = await _sut.GetStandardMasks(
                new long[] { 1111111111 });

            result.Should().ContainSingle();
            result.Single().CourseCode.Should().Be("100");

            _mockUnitOfWork.Verify(unitOfWork =>
                unitOfWork.QueryStoredProcedureAsync<CertificateMask>(
                    "Certificates_GetStandardMasks",
                    It.IsAny<DynamicParameters>(),
                    It.IsAny<int?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Then_Top_Parameter_Is_Set_To_Five()
        {
            await _sut.GetStandardMasks(
                new long[] { 1111111111 });

            _capturedParameters
                .Get<int>("@Top")
                .Should()
                .Be(5);
        }

        [Test]
        public async Task Then_A_Single_Excluded_Uln_Is_Serialized()
        {
            await _sut.GetStandardMasks(
                new long[] { 1111111111 });

            _capturedParameters
                .Get<string>("@ExcludeUlns")
                .Should()
                .Be("1111111111");
        }

        [Test]
        public async Task Then_Multiple_Excluded_Ulns_Are_Comma_Separated()
        {
            await _sut.GetStandardMasks(
                new long[]
                {
                    1111111111,
                    2222222222,
                    3333333333
                });

            _capturedParameters
                .Get<string>("@ExcludeUlns")
                .Should()
                .Be("1111111111,2222222222,3333333333");
        }

        [Test]
        public async Task Then_Null_Exclusions_Are_Passed_As_Empty()
        {
            await _sut.GetStandardMasks(null);

            _capturedParameters
                .Get<string>("@ExcludeUlns")
                .Should()
                .BeEmpty();
        }

        [Test]
        public async Task Then_Empty_Exclusions_Are_Passed_As_An_Empty_String()
        {
            await _sut.GetStandardMasks(new long[0]);

            _capturedParameters
                .Get<string>("@ExcludeUlns")
                .Should()
                .BeEmpty();
        }
    }
}