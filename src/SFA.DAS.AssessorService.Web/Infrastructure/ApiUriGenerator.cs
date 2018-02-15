namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public static class ApiUriGenerator
    {
        public static class Organisation
        {
            public static string GetOrganisation(string baseUri, int ukprn)
            {
                // Build URI for GetOrganisation call to API.
                return $"{baseUri}/{ukprn}";
            }
        }
    }
}