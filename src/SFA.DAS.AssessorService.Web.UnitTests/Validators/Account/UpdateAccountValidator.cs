using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators.Account
{
    public class UpdateAccountValidatorTests
    {
        private UpdateAccountValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new UpdateAccountValidator();
        }

        [Test]
        public void When_No_Values_Supplied_Not_Valid()
        {
            var result = _validator.Validate(new AccountViewModel());

            result.IsValid.Should().BeFalse();
            result.Errors.Find(c => c.ErrorMessage.Equals("Family name must not be empty")).Should().NotBeNull();
            result.Errors.Find(c => c.ErrorMessage.Equals("Given name must not be empty")).Should().NotBeNull();
        }

        [Test]
        public void When_Only_First_Name_Supplied_Then_Not_Valid()
        {
            var result = _validator.Validate(new AccountViewModel
            {
                GivenName = "test"
            });

            result.IsValid.Should().BeFalse();
            result.Errors.Find(c => c.ErrorMessage.Equals("Family name must not be empty")).Should().NotBeNull();
        }

        [Test]
        public void When_First_Name_And_Given_Name_Supplied_Then_Valid()
        {
            var result = _validator.Validate(new AccountViewModel
            {
                GivenName = "test",
                FamilyName = "test",
            });

            result.IsValid.Should().BeTrue();
        }
    }
}