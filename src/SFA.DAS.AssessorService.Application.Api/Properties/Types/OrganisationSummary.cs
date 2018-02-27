namespace SFA.DAS.AssessmentOrgs.Api.Client.Core.Types
{
    using System.Collections.Generic;

    public class OrganisationSummary
    {
        public string Id { get; set; }

        public string Uri { get; set; }

        public string Name { get; set; }

        public List<Link> Links { get; set; }
    }
}