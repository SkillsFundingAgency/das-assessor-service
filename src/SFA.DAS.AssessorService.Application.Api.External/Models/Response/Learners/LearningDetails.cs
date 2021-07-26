namespace SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learners
{
    using System;

    public sealed class LearningDetails : IEquatable<LearningDetails>
    {
        public string LearnerReferenceNumber { get; set; }
        public DateTime LearningStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public string ProviderName { get; set; }
        public int ProviderUkPrn { get; set; }
        public string Version { get; set; }
        public string CourseOption { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (LearnerReferenceNumber is null ? 0 : LearnerReferenceNumber.GetHashCode());
                hash = (hash * multiplier) ^ (PlannedEndDate is null ? 0 : PlannedEndDate.GetHashCode());
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
            return string.Equals(LearnerReferenceNumber, other.LearnerReferenceNumber)
                && Equals(LearningStartDate, other.LearningStartDate)
                && Equals(PlannedEndDate, other.PlannedEndDate)
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
