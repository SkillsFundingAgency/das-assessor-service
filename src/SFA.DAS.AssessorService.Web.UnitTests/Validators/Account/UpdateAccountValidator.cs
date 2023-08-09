using System.Linq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Validators;
using SFA.DAS.AssessorService.Web.ViewModels.Account;

namespace SFA.DAS.AssessorService.Web.UnitTests.Validators.Account;

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
        
        Assert.IsFalse(result.IsValid);
        Assert.IsNotNull(result.Errors.FirstOrDefault(c => c.ErrorMessage.Equals("Family name must not be empty")));
        Assert.IsNotNull(result.Errors.FirstOrDefault(c => c.ErrorMessage.Equals("Given name must not be empty")));
    }

    [Test]
    public void When_Only_First_Name_Supplied_Then_Not_Valid()
    {
        var result = _validator.Validate(new AccountViewModel
        {
            GivenName = "test"
        });
        
        Assert.IsFalse(result.IsValid);
        Assert.IsNotNull(result.Errors.FirstOrDefault(c => c.ErrorMessage.Equals("Family name must not be empty")));
    }
    
    [Test]
    public void When_First_Name_And_Given_Name_Supplied_Then_Valid()
    {
        var result = _validator.Validate(new AccountViewModel
        {
            GivenName = "test",
            FamilyName = "test",
        });
        
        Assert.IsTrue(result.IsValid);
    }
}