namespace SFA.DAS.AssessorService.Web.Staff.Tests.Validators
{
    using System;
    using System.Linq;
    using Api.Types.Models.Roatp;
    using FluentAssertions;
    using Infrastructure;
    using Moq;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.Web.Staff.Resources;
    using SFA.DAS.AssessorService.Web.Staff.Validators.Roatp;
    using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;

    [TestFixture]
    public class AddOrganisationValidatorTests
    {
        private AddOrganisationViewModel _viewModel;
        private AddOrganisationValidator _validator;
        private Mock<IRoatpApiClient> _apiClient;

        [SetUp]
        public void Before_each_test()
        {
            _apiClient = new Mock<IRoatpApiClient>();
            _apiClient.Setup(x => x.DuplicateUKPRNCheck(It.IsAny<Guid>(), It.IsAny<long>()))
                .ReturnsAsync(new DuplicateCheckResponse {DuplicateFound = false, DuplicateOrganisationName = null});
            _apiClient.Setup(x => x.DuplicateCompanyNumberCheck(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new DuplicateCheckResponse { DuplicateFound = false, DuplicateOrganisationName = null });
            _apiClient.Setup(x => x.DuplicateCharityNumberCheck(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new DuplicateCheckResponse { DuplicateFound = false, DuplicateOrganisationName = null });
            
            _validator = new AddOrganisationValidator(_apiClient.Object, new RoatpOrganisationValidator());

            _viewModel = new AddOrganisationViewModel
            {
                LegalName = "Legal name",
                ProviderTypeId = 1,
                CharityNumber = "11223311-1",
                CompanyNumber = "AB123456",
                TradingName = "Trading Name",
                OrganisationTypeId = 0,
                UKPRN = "10002000",
                OrganisationId = Guid.NewGuid()
            };

        }

        [TestCase("")]
        [TestCase(null)]
        public void Validator_rejects_missing_legal_name(string legalName)
        {
            _viewModel.LegalName = legalName;

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var legalNameError = validationResponse.Errors.FirstOrDefault(x => x.Field == "LegalName");

            legalNameError.Should().NotBeNull();
            legalNameError.ErrorMessage.Should().Be(RoatpOrganisationValidation.LegalNameMandatory);
        }

        [Test]
        public void Validator_rejects_legal_name_too_long()
        {
            _viewModel.LegalName = new string('A', 201);

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var legalNameError = validationResponse.Errors.FirstOrDefault(x => x.Field == "LegalName");

            legalNameError.Should().NotBeNull();
            legalNameError.ErrorMessage.Should().Be(RoatpOrganisationValidation.LegalNameMaxLength);
        }

        [Test]
        public void Validator_rejects_legal_name_too_short()
        {
            _viewModel.LegalName = "A";

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var legalNameError = validationResponse.Errors.FirstOrDefault(x => x.Field == "LegalName");

            legalNameError.Should().NotBeNull();
            legalNameError.ErrorMessage.Should().Be(RoatpOrganisationValidation.LegalNameMinLength);
        }

        [Test]
        public void Validator_rejects_trading_name_too_long()
        {
            _viewModel.TradingName = new string('A', 201);

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var tradingNameError = validationResponse.Errors.FirstOrDefault(x => x.Field == "TradingName");

            tradingNameError.Should().NotBeNull();
            tradingNameError.ErrorMessage.Should().Be(RoatpOrganisationValidation.TradingNameMaxLength);
        }

        [TestCase(1000001)]
        [TestCase(999999)]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(100000001)]
        public void Validator_rejects_invalid_UKPRN_values(long ukprn)
        {
            _viewModel.UKPRN = ukprn.ToString();

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var ukprnError = validationResponse.Errors.FirstOrDefault(x => x.Field == "UKPRN");

            ukprnError.Should().NotBeNull();
            ukprnError.ErrorMessage.Should().Be(RoatpOrganisationValidation.UKPRNLength);
        }

        [TestCase("A1234567")]
        [TestCase("1 234567")]
        public void Validator_rejects_invalid_characters_in_UKPRN(string ukprn)
        {
            _viewModel.UKPRN = ukprn;


            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var ukprnError = validationResponse.Errors.FirstOrDefault(x => x.Field == "UKPRN");

            ukprnError.Should().NotBeNull();
            ukprnError.ErrorMessage.Should().Be(RoatpOrganisationValidation.UKPRNFormat);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Validator_rejects_null_or_empty_UKPRN(string ukprn)
        {
            _viewModel.UKPRN = ukprn;

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var ukprnError = validationResponse.Errors.FirstOrDefault(x => x.Field == "UKPRN");

            ukprnError.Should().NotBeNull();
            ukprnError.ErrorMessage.Should().Be(RoatpOrganisationValidation.UKPRNMandatory);
        }

        [Test]
        public void Validator_rejects_UKPRN_if_already_used()
        {
            _viewModel.UKPRN = "11112222";
            _apiClient.Setup(x => x.DuplicateUKPRNCheck(It.IsAny<Guid>(), It.IsAny<long>()))
                .ReturnsAsync(new DuplicateCheckResponse
                {
                    DuplicateFound = true,
                    DuplicateOrganisationName = "Test Ltd"
                });


            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var ukprnError = validationResponse.Errors.FirstOrDefault(x => x.Field == "UKPRN");

            ukprnError.Should().NotBeNull();
            string expectedMessage = string.Format(RoatpOrganisationValidation.UKPRNDuplicateMatch, "Test Ltd");
            ukprnError.ErrorMessage.Should().Be(expectedMessage);
        }


        [TestCase("ABC12345")]
        [TestCase("!£$%^&*(")]
        [TestCase("A1234567")]
        [TestCase("ab123456")]
        public void Validator_rejects_invalid_company_number(string companyNumber)
        {
            _viewModel.CompanyNumber = companyNumber;

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var companyNumberError = validationResponse.Errors.FirstOrDefault(x => x.Field == "CompanyNumber");

            companyNumberError.Should().NotBeNull();
            companyNumberError.ErrorMessage.Should().Be(RoatpOrganisationValidation.CompanyNumberFormat);
        }

        [TestCase("1234567")]
        [TestCase("012345678")]
        [TestCase("1000$!&*^%")]
        public void Validator_rejects_invalid_company_number_length(string companyNumber)
        {
            _viewModel.CompanyNumber = companyNumber;

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var companyNumberError = validationResponse.Errors.FirstOrDefault(x => x.Field == "CompanyNumber");

            companyNumberError.Should().NotBeNull();
            companyNumberError.ErrorMessage.Should().Be(RoatpOrganisationValidation.CompanyNumberLength);
        }

        [Test]
        public void Validator_rejects_company_number_if_already_used()
        {
            _viewModel.CompanyNumber = "12345678";
            _apiClient.Setup(x => x.DuplicateCompanyNumberCheck(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new DuplicateCheckResponse
                {
                    DuplicateFound = true,
                    DuplicateOrganisationName = "Test Ltd"
                });

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var companyNumberError = validationResponse.Errors.FirstOrDefault(x => x.Field == "CompanyNumber");

            companyNumberError.Should().NotBeNull();
            string expectedMessage = string.Format(RoatpOrganisationValidation.CompanyNumberDuplicateMatch, "Test Ltd");
            companyNumberError.ErrorMessage.Should().Be(expectedMessage);
        }

        [TestCase("1000$!&*^%")]
        [TestCase("!£$%^&*()")]
        public void Validator_rejects_invalid_charity_number(string charityNumber)
        {
            _viewModel.CharityNumber = charityNumber;

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var charityNumberError = validationResponse.Errors.FirstOrDefault(x => x.Field == "CharityNumber");

            charityNumberError.Should().NotBeNull();
            charityNumberError.ErrorMessage.Should().Be(RoatpOrganisationValidation.CharityNumberFormat);
        }

        [TestCase("010101888-1££££''''")]
        public void Validator_rejects_charity_number_with_invalid_length(string charityNumber)
        {
            _viewModel.CharityNumber = charityNumber;

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var charityNumberError = validationResponse.Errors.FirstOrDefault(x => x.Field == "CharityNumber");

            charityNumberError.Should().NotBeNull();
            charityNumberError.ErrorMessage.Should().Be(RoatpOrganisationValidation.CharityNumberLength);
        }

        [Test]
        public void Validator_rejects_charity_number_if_already_used()
        {
            _viewModel.CharityNumber = "12345678";
            _apiClient.Setup(x => x.DuplicateCharityNumberCheck(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(new DuplicateCheckResponse
                {
                    DuplicateFound = true,
                    DuplicateOrganisationName = "Test Ltd"
                });

            var validationResponse = _validator.ValidateOrganisationDetails(_viewModel).GetAwaiter().GetResult();

            var charityNumberError = validationResponse.Errors.FirstOrDefault(x => x.Field == "CharityNumber");

            charityNumberError.Should().NotBeNull();
            string expectedMessage = string.Format(RoatpOrganisationValidation.CharityNumberDuplicateMatch, "Test Ltd");
            charityNumberError.ErrorMessage.Should().Be(expectedMessage);
        }
    }
}
