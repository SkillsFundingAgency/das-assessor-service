namespace SFA.DAS.AssessorService.Web.Staff.Tests.Validators
{
    using FluentAssertions;
    using NUnit.Framework;
    using SFA.DAS.AssessorService.Web.Staff.Resources;
    using Staff.Validators.Roatp;
    using System.Linq;

    [TestFixture]
    public class RoatpOrganisationValidatorTests
    {
        private RoatpOrganisationValidator _validator;

        [SetUp]
        public void Before_each_test()
        {
            _validator = new RoatpOrganisationValidator();
        }

        [TestCase("")]
        [TestCase(null)]
        public void Validator_rejects_missing_legal_name(string legalName)
        {
            var validationErrors = _validator.IsValidLegalName(legalName);

            var legalNameError = validationErrors.FirstOrDefault(x => x.Field == "LegalName");

            legalNameError.Should().NotBeNull();
            legalNameError.ErrorMessage.Should().Be(RoatpOrganisationValidation.LegalNameMandatory);
        }

        [Test]
        public void Validator_rejects_legal_name_too_long()
        {
            var legalName = new string('A', 201);

            var validationErrors = _validator.IsValidLegalName(legalName);

            var legalNameError = validationErrors.FirstOrDefault(x => x.Field == "LegalName");

            legalNameError.Should().NotBeNull();
            legalNameError.ErrorMessage.Should().Be(RoatpOrganisationValidation.LegalNameMaxLength);
        }

        [Test]
        public void Validator_rejects_legal_name_too_short()
        {
            var legalName = "A";

            var validationErrors = _validator.IsValidLegalName(legalName);

            var legalNameError = validationErrors.FirstOrDefault(x => x.Field == "LegalName");

            legalNameError.Should().NotBeNull();
            legalNameError.ErrorMessage.Should().Be(RoatpOrganisationValidation.LegalNameMinLength);
        }

        [Test]
        public void Validator_rejects_trading_name_too_long()
        {
            var tradingName = new string('A', 201);

            var validationErrors = _validator.IsValidTradingName(tradingName);

            var tradingNameError = validationErrors.FirstOrDefault(x => x.Field == "TradingName");

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
            var validationErrors = _validator.IsValidUKPRN(ukprn.ToString());

            var ukprnError = validationErrors.FirstOrDefault(x => x.Field == "UKPRN");

            ukprnError.Should().NotBeNull();
            ukprnError.ErrorMessage.Should().Be(RoatpOrganisationValidation.UKPRNLength);
        }

        [TestCase("A1234567")]
        [TestCase("1 234567")]
        public void Validator_rejects_invalid_characters_in_UKPRN(string ukprn)
        {
            var validationErrors = _validator.IsValidUKPRN(ukprn);

            var ukprnError = validationErrors.FirstOrDefault(x => x.Field == "UKPRN");

            ukprnError.Should().NotBeNull();
            ukprnError.ErrorMessage.Should().Be(RoatpOrganisationValidation.UKPRNFormat);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void Validator_rejects_null_or_empty_UKPRN(string ukprn)
        {
            var validationErrors = _validator.IsValidUKPRN(ukprn);

            var ukprnError = validationErrors.FirstOrDefault(x => x.Field == "UKPRN");

            ukprnError.Should().NotBeNull();
            ukprnError.ErrorMessage.Should().Be(RoatpOrganisationValidation.UKPRNMandatory);
        }

        [TestCase("ABC12345")]
        [TestCase("!£$%^&*(")]
        [TestCase("A1234567")]
        [TestCase("ab123456")]
        public void Validator_rejects_invalid_company_number(string companyNumber)
        {
            var validationErrors = _validator.IsValidCompanyNumber(companyNumber);

            var companyNumberError = validationErrors.FirstOrDefault(x => x.Field == "CompanyNumber");

            companyNumberError.Should().NotBeNull();
            companyNumberError.ErrorMessage.Should().Be(RoatpOrganisationValidation.CompanyNumberFormat);
        }

        [TestCase("1234567")]
        [TestCase("012345678")]
        [TestCase("1000$!&*^%")]
        public void Validator_rejects_invalid_company_number_length(string companyNumber)
        {
            var validationErrors = _validator.IsValidCompanyNumber(companyNumber);

            var companyNumberError = validationErrors.FirstOrDefault(x => x.Field == "CompanyNumber");

            companyNumberError.Should().NotBeNull();
            companyNumberError.ErrorMessage.Should().Be(RoatpOrganisationValidation.CompanyNumberLength);
        }

        [TestCase("1000$!&*^%")]
        [TestCase("!£$%^&*()")]
        public void Validator_rejects_invalid_charity_number(string charityNumber)
        {
            var validationErrors = _validator.IsValidCharityNumber(charityNumber);

            var charityNumberError = validationErrors.FirstOrDefault(x => x.Field == "CharityNumber");

            charityNumberError.Should().NotBeNull();
            charityNumberError.ErrorMessage.Should().Be(RoatpOrganisationValidation.CharityNumberFormat);
        }

        [TestCase("010101888-1££££''''")]
        public void Validator_rejects_charity_number_with_invalid_length(string charityNumber)
        {
            var validationErrors = _validator.IsValidCharityNumber(charityNumber);

            var charityNumberError = validationErrors.FirstOrDefault(x => x.Field == "CharityNumber");

            charityNumberError.Should().NotBeNull();
            charityNumberError.ErrorMessage.Should().Be(RoatpOrganisationValidation.CharityNumberLength);
        }
    }
}
