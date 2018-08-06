using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Certificates
{
    public class LearningDetails : IEquatable<LearningDetails>
    {
        public int StandardCode { get; set; }
        public string StandardName { get; set; }
        public int StandardLevel { get; set; }
        public DateTime StandardPublicationDate { get; set; }
        public string CourseOption { get; set; }
        public string OverallGrade { get; set; }
        public string AchievementOutcome { get; set; }
        public DateTime? AchievementDate { get; set; }
        public DateTime LearningStartDate { get; set; }
        public string ProviderName { get; set; }
        public int ProviderUkPrn { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ StandardCode.GetHashCode();
                hash = (hash * multiplier) ^ (StandardName is null ? 0 : StandardName.GetHashCode());
                hash = (hash * multiplier) ^ StandardLevel.GetHashCode();
                hash = (hash * multiplier) ^ StandardPublicationDate.GetHashCode();
                hash = (hash * multiplier) ^ (CourseOption is null ? 0 : CourseOption.GetHashCode());
                hash = (hash * multiplier) ^ (OverallGrade is null ? 0 : OverallGrade.GetHashCode());
                hash = (hash * multiplier) ^ (AchievementOutcome is null ? 0 : AchievementOutcome.GetHashCode());
                hash = (hash * multiplier) ^ (AchievementDate is null ? 0 : AchievementDate.GetHashCode());
                hash = (hash * multiplier) ^ LearningStartDate.GetHashCode();
                hash = (hash * multiplier) ^ (ProviderName is null ? 0 : ProviderName.GetHashCode());
                hash = (hash * multiplier) ^ ProviderUkPrn.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((LearningDetails)obj);
        }

        public bool Equals(LearningDetails other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(LearningDetails other)
        {
            return Equals(StandardCode, other.StandardCode)
                && string.Equals(StandardName, other.StandardName)
                && Equals(StandardLevel, other.StandardLevel)
                && Equals(StandardPublicationDate, other.StandardPublicationDate)
                && string.Equals(CourseOption, other.CourseOption)
                && string.Equals(OverallGrade, other.OverallGrade)
                && string.Equals(AchievementOutcome, other.AchievementOutcome)
                && Equals(AchievementDate, other.AchievementDate)
                && Equals(LearningStartDate, other.LearningStartDate)
                && string.Equals(ProviderName, other.ProviderName)
                && Equals(ProviderUkPrn, other.ProviderUkPrn);
        }

        public static bool operator ==(LearningDetails left, LearningDetails right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(LearningDetails left, LearningDetails right)
        {
            return !(left == right);
        }
        #endregion
    }
}
