using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Validators;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.Register
{
    [TestFixture]
    public class EpaORganisationSearchValidatorTests
    {
        private EpaOrganisationSearchValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new EpaOrganisationSearchValidator();
        }

        [TestCase("EPA0000", true, null)]
        [TestCase("EPA000", false, "checking there is at least 4 numbers after the 'epa'")]
        [TestCase("epa0000", true, null)]
        [TestCase("EPA0000A", false, null)]
        [TestCase("EPA000000000", true, null)]
        [TestCase("EPA0000000000", false, "checking it is not greated that 12 characters in all")]
        [TestCase("stuff", false, null)]
        [TestCase("", false, null)]
        public void CheckEpaOrganisationIdIsAcceptable(string organisationIdToCheck, bool matching, string message)
        {
            var result = _validator.IsValidEpaOrganisationId(organisationIdToCheck);
            if (message != null)
                Assert.AreEqual(matching, result, message);
            else
                Assert.AreEqual(matching, result);
        }

        [TestCase("10000000", true)]
        [TestCase("99999999", true)]
        [TestCase("099999999", true)]
        [TestCase("100000000", false)]
        [TestCase("09999999", false)]
        [TestCase("test", false)]
        [TestCase("testtest", false)]
        public void CheckUkprnIsValid(string stringToCheck, bool matching)
        {
            var result = _validator.IsValidUkprn(stringToCheck);
            Assert.AreEqual(matching, result);
        }
    }
}
