using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Response.Learners
{
    public sealed class LearnerData : IEquatable<LearnerData>
    {
        public Certificates.Standard Standard { get; set; }
        public Certificates.Learner Learner { get; set; }
        public LearningDetails LearningDetails { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (Standard is null ? 0 : Standard.GetHashCode());
                hash = (hash * multiplier) ^ (Learner is null ? 0 : Learner.GetHashCode());
                hash = (hash * multiplier) ^ (LearningDetails is null ? 0 : LearningDetails.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((LearnerData)obj);
        }

        public bool Equals(LearnerData other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(LearnerData other)
        {
            return Equals(Standard, other.Standard)
                && Equals(Learner, other.Learner)
                && Equals(LearningDetails, other.LearningDetails);
        }

        public static bool operator ==(LearnerData left, LearnerData right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(LearnerData left, LearnerData right)
        {
            return !(left == right);
        }
        #endregion
    }
}
