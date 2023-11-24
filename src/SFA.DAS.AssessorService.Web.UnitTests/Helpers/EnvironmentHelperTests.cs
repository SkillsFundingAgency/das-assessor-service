using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Helpers;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers;

public class EnvironmentHelperTests
{
    [TestCase("TEST","test-assessors.apprenticeships.education.gov.uk")]
    [TestCase("pRd","assessors.apprenticeships.education.gov.uk")]
    public void Then_The_Domain_Is_Resolved_For_The_Environment(string environment, string expected)
    {
        var actual = EnvironmentHelper.GetDomain(environment);
        
        Assert.AreEqual(expected, actual);
    }
}