using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Validators.SearchRequestValidator
{
    //[TestFixture]
    //public class WhenRequestTypeIsDobSurname
    //{
    //    private Api.Validators.SearchRequestValidator _validator;

    //    [SetUp]
    //    public void Arrange()
    //    {
    //        _validator = new Api.Validators.SearchRequestValidator();
    //    }

    //    [Test]
    //    public void ThenValidatorReturnsFalseIfMissingDateOfBirthAndSurname()
    //    {
    //        var result = _validator.Validate(new SearchQueryViewModel {SearchType = SearchTypes.DobSurname});
    //        result.IsValid.Should().BeFalse();
    //    }

    //    [Test]
    //    public void ThenValidatorReturnsFalseIfMissingDateOfBirth()
    //    {
    //        var result = _validator.Validate(new SearchQueryViewModel { SearchType = SearchTypes.DobSurname, Surname = "Gouge"});
    //        result.IsValid.Should().BeFalse();
    //    }

    //    [Test]
    //    public void ThenValidatorReturnsFalseIfMissingSurname()
    //    {
    //        var result = _validator.Validate(new SearchQueryViewModel
    //        {
    //            SearchType = SearchTypes.DobSurname,
    //            DobDay = 1,
    //            DobMonth = 1,
    //            DobYear = 1977
    //        });
    //        result.IsValid.Should().BeFalse();
    //    }

    //    [Test]
    //    public void ThenValidatorReturnsTrueIfDateOfBirthAndSurnamePresent()
    //    {
    //        var result = _validator.Validate(new SearchQueryViewModel
    //        {
    //            SearchType = SearchTypes.DobSurname,
    //            Uln = "9123671552",
    //            DobDay = 1,
    //            DobMonth = 1,
    //            DobYear = 1977,
    //            Surname = "Gouge"
    //        });
    //        result.IsValid.Should().BeTrue();
    //    }
    //}
}