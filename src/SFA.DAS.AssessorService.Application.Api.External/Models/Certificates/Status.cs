using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Certificates
{
    public class Status : IEquatable<Status>
    {
        public string CurrentStatus { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (CurrentStatus is null ? 0 : CurrentStatus.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((Status)obj);
        }

        public bool Equals(Status other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(Status other)
        {
            return string.Equals(CurrentStatus, other.CurrentStatus);
        }

        public static bool operator ==(Status left, Status right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Status left, Status right)
        {
            return !(left == right);
        }
        #endregion
    }
}
