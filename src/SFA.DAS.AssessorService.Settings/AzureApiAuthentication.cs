using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class AzureApiAuthentication : IAzureApiAuthentication
    {
        //[JsonRequired]
        public string Id { get; set; } = "integration";

        //[JsonRequired]
        public string Key { get; set; } = "M0eiHVU1PeHymGUFbRa1OCWwbfonn4KD036fRpSPG9qMatQ1Vp6O6QXbQ93us+QZEAa0jJZ13bV+a86UybXWag==";

        //[JsonRequired]
        public string ApiBaseAddress { get; set; } = @"https://apim-mh-poc.management.azure-api.net";
    }
}