using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Application.Api.Validators;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Organisations.EpaOrganisation
{
    public class WhenValidateEpaOrganisation
    {
        private Mock<IRegisterValidationRepository> _registerRepositoryMock;
        private Mock<IRegisterQueryRepository> _registerQueryRepositoryMock;
        private Mock<IStringLocalizer<EpaOrganisationValidator>> _localizerMock;
        private Mock<ISpecialCharacterCleanserService> _cleanserServiceMock;
        private Mock<IStandardService> _standardServiceMock;
        private EpaOrganisationValidator _epaOrganisationValidator;
        private UpdateEpaOrganisationRequest _request;

        [SetUp]
        public void Setup()
        {
            _registerRepositoryMock = new Mock<IRegisterValidationRepository>();
            _registerQueryRepositoryMock = new Mock<IRegisterQueryRepository>();
            _localizerMock = new Mock<IStringLocalizer<EpaOrganisationValidator>>();
            _cleanserServiceMock = new Mock<ISpecialCharacterCleanserService>();
            _standardServiceMock = new Mock<IStandardService>();

            _epaOrganisationValidator = new EpaOrganisationValidator(
                _registerRepositoryMock.Object,
                _registerQueryRepositoryMock.Object, 
                _cleanserServiceMock.Object, 
                _localizerMock.Object, 
                _standardServiceMock.Object);;

            _request = new UpdateEpaOrganisationRequest
            {
                ActionChoice = "MakeLive",
                CompanyNumber = "AA000000",
                OrganisationId = "123",
                CompanySummary = new ApplyTypes.CompaniesHouse.CompaniesHouseSummary(),
                CharityNumber = "AA000000",
                Name = "TestName",
                Ukprn = 12345679,
                OrganisationTypeId = 123,
                Address1 = "FirstLine",
                Address2 = "SecondLine",
                Address3 = "ThirdLine",
                Address4 = "FourthLine",
                Postcode = "CV1 2AT",
                RecognitionNumber = "RN123"
            };
        }

        [Test, MoqAutoData]
        public void ThenSucceedsValidationWithNoStandardLookup()
        {
            // Arrange
            _registerRepositoryMock.Setup(x => x.EpaOrganisationExistsWithOrganisationId(_request.OrganisationId))
                .ReturnsAsync(true);

            _registerRepositoryMock.Setup(x => x.EpaOrganisationExistsWithCompanyNumber(_request.OrganisationId, _request.CompanyNumber))
                .ReturnsAsync(false);

            _registerRepositoryMock.Setup(x => x.EpaOrganisationExistsWithCharityNumber(_request.OrganisationId, _request.CharityNumber))
                .ReturnsAsync(false);

            _cleanserServiceMock.Setup(x => x.CleanseStringForSpecialCharacters(_request.Name)).Returns(_request.Name);

            _registerRepositoryMock.Setup(x => x.EpaOrganisationAlreadyUsingUkprn(_request.Ukprn.Value, _request.OrganisationId))
                .ReturnsAsync(false);

            _registerRepositoryMock.Setup(x => x.EpaOrganisationAlreadyUsingName(_request.Name, _request.OrganisationId))
                .ReturnsAsync(false);

            _registerRepositoryMock.Setup(x => x.OrganisationTypeExists(_request.OrganisationTypeId.Value))
                .ReturnsAsync(true);

            _registerRepositoryMock.Setup(x => x.EpaOrganisationExistsWithRecognitionNumber(_request.RecognitionNumber.ToLower(), _request.OrganisationId.ToLower()))
                .ReturnsAsync(false);

            _registerRepositoryMock.Setup(x => x.CheckRecognitionNumberExists(_request.RecognitionNumber.ToLower()))
                .ReturnsAsync(true);

            _cleanserServiceMock.Setup(x => x.CleanseStringForSpecialCharacters(_request.Address1)).Returns(_request.Address1);
            _cleanserServiceMock.Setup(x => x.CleanseStringForSpecialCharacters(_request.Address2)).Returns(_request.Address2);
            _cleanserServiceMock.Setup(x => x.CleanseStringForSpecialCharacters(_request.Address3)).Returns(_request.Address3);
            _cleanserServiceMock.Setup(x => x.CleanseStringForSpecialCharacters(_request.Address4)).Returns(_request.Address4);

            _cleanserServiceMock.Setup(x => x.CleanseStringForSpecialCharacters(_request.Postcode)).Returns(_request.Postcode);

            _registerQueryRepositoryMock.Setup(x => x.GetAssessmentOrganisationContacts(_request.OrganisationId))
                .ReturnsAsync(new List<AssessmentOrganisationContact> { new AssessmentOrganisationContact() });

            // Act
            var result = _epaOrganisationValidator.ValidatorUpdateEpaOrganisationRequest(_request);

            // Assert
            result.Errors.Should().BeEmpty();
            result.IsValid.Should().BeTrue();
        }
    }
}
