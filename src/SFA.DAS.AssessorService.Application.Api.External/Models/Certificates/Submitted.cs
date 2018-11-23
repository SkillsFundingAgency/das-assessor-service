using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Certificates
{
    public class Submitted : IEquatable<Submitted>
    {
        public DateTime? SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (SubmittedAt is null ? 0 : SubmittedAt.GetHashCode());
                hash = (hash * multiplier) ^ (SubmittedBy is null ? 0 : SubmittedBy.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((Submitted)obj);
        }

        public bool Equals(Submitted other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(Submitted other)
        {
            return Equals(SubmittedAt, other.SubmittedAt)
                && string.Equals(SubmittedBy, other.SubmittedBy);
        }

        public static bool operator ==(Submitted left, Submitted right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Submitted left, Submitted right)
        {
            return !(left == right);
        }
        #endregion
    }
}
