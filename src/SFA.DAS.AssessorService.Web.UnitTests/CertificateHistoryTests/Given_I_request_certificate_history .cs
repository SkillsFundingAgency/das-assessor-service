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
    public class Given_I_request_certificate_history
    {       
        public void Then_certificate_history_should_return_valid_records()
        {

            //var localizer = new Mock<IStringLocalizer<SearchQueryViewModelValidator>>();

            //localizer.Setup(l => l[ResourceKey.LastNameShouldNotBeEmpty])
            //    .Returns(new LocalizedString(ResourceKey.LastNameShouldNotBeEmpty, "Last name should not be empty"));
            //localizer.Setup(l => l[ResourceKey.UlnFormatInvalid])
            //    .Returns(new LocalizedString(ResourceKey.UlnFormatInvalid, "Uln format invalid"));
            //localizer.Setup(l => l[ResourceKey.UlnShouldNotBeEmpty])
            //    .Returns(new LocalizedString(ResourceKey.UlnShouldNotBeEmpty, "ULN should not be empty"));

            //var validator = new SearchQueryViewModelValidator(localizer.Object);
            //var result = validator.Validate(new SearchRequestViewModel() {Surname = "Smith", Uln = invalidUln});
            //result.IsValid.Should().BeFalse();
        }
    }
}