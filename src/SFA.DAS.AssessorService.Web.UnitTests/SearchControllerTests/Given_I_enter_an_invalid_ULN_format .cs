using FluentAssertions;
using Microsoft.Extensions.Localization;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Search;

namespace SFA.DAS.AssessorService.Web.UnitTests.SearchControllerTests
{
    [TestFixture]
    public class Given_I_enter_an_invalid_ULN_format 
    {
        [TestCase("123456789")]
        [TestCase("12345678901")]
        public void Then_search_validation_should_fail(string invalidUln)
        {
            var localizer = new Mock<IStringLocalizer<SearchQueryViewModelValidator>>();

            localizer.Setup(l => l[ResourceKey.LastNameShouldNotBeEmpty])
                .Returns(new LocalizedString(ResourceKey.LastNameShouldNotBeEmpty, "Last name should not be empty"));
            localizer.Setup(l => l[ResourceKey.UlnFormatInvalid])
                .Returns(new LocalizedString(ResourceKey.UlnFormatInvalid, "Uln format invalid"));
            
            var validator = new SearchQueryViewModelValidator(localizer.Object);
            var result = validator.Validate(new SearchViewModel() {Surname = "Smith", Uln = invalidUln});
            result.IsValid.Should().BeFalse();
        }
    }
}