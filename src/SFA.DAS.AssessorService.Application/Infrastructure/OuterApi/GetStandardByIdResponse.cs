using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class StandardDetailResponse
    {
        [JsonProperty("standardUId")]
        public string StandardUId { get; set; }

        [JsonProperty("larsCode")]
        public int LarsCode { get; set; }

        [JsonProperty("ifateReferenceNumber")]
        public string IfateReferenceNumber { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("coronationEmblem")]
        public bool CoronationEmblem { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("versionMajor")]
        public int VersionMajor { get; set; }
        
        [JsonProperty("versionMinor")]
        public int VersionMinor { get; set; }

        [JsonProperty("overviewOfRole")]
        public string OverviewOfRole { get; set; }

        [JsonProperty("keywords")]
        public string Keywords { get; set; }

        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonProperty("assessmentPlanUrl")]
        public string AssessmentPlanUrl { get; set; }

        [JsonProperty("trailBlazerContact")]
        public string TrailBlazerContact { get; set; }

        [JsonProperty("standardPageUrl")]
        public string StandardPageUrl { get; set; }

        [JsonProperty("integratedDegree")]
        public string IntegratedDegree { get; set; }

        [JsonProperty("sectorSubjectAreaTier2")]
        public decimal SectorSubjectAreaTier2 { get; set; }

        [JsonProperty("sectorSubjectAreaTier2Description")]
        public string SectorSubjectAreaTier2Description { get; set; }

        [JsonProperty("standardDates")]
        public StandardDates StandardDates { get; set; }

        [JsonProperty("versionDetail")]
        public StandardVersionDetail VersionDetail { get; set; }

        [JsonProperty("eqaProvider")]
        public EqaProvider EqaProvider { get; set; }

        [JsonProperty("options")]
        public List<string> Options { get; set; }

        [JsonProperty("typicalDuration")]
        public int TypicalDuration { get; set; }

        [JsonProperty("maxFunding")]
        public int MaxFunding { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("epaChanged")]
        public bool EPAChanged { get; set; }
    }

    public class StandardDates
    {
        [JsonProperty("lastDateStarts")]
        public DateTime? LastDateStarts { get; set; }

        [JsonProperty("effectiveTo")]
        public DateTime? EffectiveTo { get; set; }

        [JsonProperty("effectiveFrom")]
        public DateTime EffectiveFrom { get; set; }
    }

    public class EqaProvider
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("contactName")]
        public string ContactName { get; set; }

        [JsonProperty("contactEmail")]
        public string ContactEmail { get; set; }
        
        [JsonProperty("webLink")]
        public string WebLink { get; set; }
    }

    public class StandardVersionDetail
    {
        [JsonProperty("earliestStartDate")]
        public DateTime? EarliestStartDate { get; set; }

        [JsonProperty("latestStartDate")]
        public DateTime? LatestStartDate { get; set; }

        [JsonProperty("latestEndDate")]
        public DateTime? LatestEndDate { get; set; }

        [JsonProperty("approvedForDelivery")]
        public DateTime? ApprovedForDelivery { get; set; }

        [JsonProperty("proposedTypicalDuration")]
        public int ProposedTypicalDuration { get; set; }

        [JsonProperty("proposedMaxFunding")]
        public int ProposedMaxFunding { get; set; }
    }
}