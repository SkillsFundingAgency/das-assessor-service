using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.JsonData
{
    public class CertificateData
    {
        public string LearnerGivenNames { get; set; }
        public string LearnerFamilyName { get; set; }
        public long? EmployerAccountId { get; set; }
        public string EmployerName { get; set; }
        public string StandardReference { get; set; }
        public string Version { get; set; }
        public string StandardName { get; set; }
        public int StandardLevel { get; set; }
        public DateTime? StandardPublicationDate { get; set; }
        public string ContactName { get; set; }
        public string ContactOrganisation { get; set; }
        public string ContactAddLine1 { get; set; }
        public string ContactAddLine2 { get; set; }
        public string ContactAddLine3 { get; set; }
        public string ContactAddLine4 { get; set; }
        public string ContactPostCode { get; set; }
        public string ProviderName { get; set; }
        public string Registration { get; set; }
        public DateTime? LearningStartDate { get; set; }
        public DateTime? AchievementDate { get; set; }
        public string CourseOption { get; set; }
        public string OverallGrade { get; set; }
        public string Department { get; set; }
        public string FullName { get; set; }
        public string IncidentNumber { get; set; }
        public EpaDetails EpaDetails { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CertificateSendTo SendTo { get; set; }
        public List<string> ReprintReasons { get; set; }
        public List<string> AmendReasons { get; set; }
    }

    public class EpaDetails
    {
        public string EpaReference { get; set; }
        public DateTime? LatestEpaDate { get; set; }
        public string LatestEpaOutcome { get; set; }
        public List<EpaRecord> Epas { get; set; }
    }

    public class EpaRecord
    {
        public DateTime EpaDate { get; set; }
        public string EpaOutcome { get; set; }
        public bool? Resit { get; set; }
        public bool? Retake { get; set; }
    }

    public enum CertificateSendTo
    {
        None,
        Apprentice,
        Employer
    }

    public class CertificateSendToEnumConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (string.IsNullOrEmpty(reader.Value.ToString()))
                return CertificateSendTo.None;

            return base.ReadJson(reader, objectType, existingValue, serializer);
        }
    }
}