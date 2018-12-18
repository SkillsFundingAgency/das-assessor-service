using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Certificates
{
    public class Created : IEquatable<Created>
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ CreatedAt.GetHashCode();
                hash = (hash * multiplier) ^ (CreatedBy is null ? 0 : CreatedBy.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((Created)obj);
        }

        public bool Equals(Created other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(Created other)
        {
            return Equals(CreatedAt, other.CreatedAt)
                && string.Equals(CreatedBy, other.CreatedBy);
        }

        public static bool operator ==(Created left, Created right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Created left, Created right)
        {
            return !(left == right);
        }
        #endregion
    }
}
