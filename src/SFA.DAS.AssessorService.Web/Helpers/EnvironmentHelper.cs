namespace SFA.DAS.AssessorService.Web.Helpers;

public static class EnvironmentHelper
{
    public static string GetDomain(string environment)
    {
        var environmentPart = environment.ToLower() == "prd" ? "assessors" : $"{environment.ToLower()}-assessors";
        return $"{environmentPart}.apprenticeships.education.gov.uk";
    }
}