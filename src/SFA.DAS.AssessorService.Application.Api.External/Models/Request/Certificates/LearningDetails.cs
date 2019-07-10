using SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Attributes;
using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Request.Certificates
{
    public class LearningDetails : IEquatable<LearningDetails>
    {
        public string CourseOption { get; set; }
        [SwaggerRequired]
        public string OverallGrade { get; set; }
        [SwaggerRequired]
        public DateTime AchievementDate { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (CourseOption is null ? 0 : CourseOption.GetHashCode());
                hash = (hash * multiplier) ^ (OverallGrade is null ? 0 : OverallGrade.GetHashCode());
                hash = (hash * multiplier) ^ AchievementDate.GetHashCode();
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
            return string.Equals(CourseOption, other.CourseOption)
                && string.Equals(OverallGrade, other.OverallGrade)
                && Equals(AchievementDate, other.AchievementDate);
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
