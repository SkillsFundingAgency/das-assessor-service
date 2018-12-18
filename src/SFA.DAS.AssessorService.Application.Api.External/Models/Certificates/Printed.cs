using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Certificates
{
    public class Printed : IEquatable<Printed>
    {
        public DateTime? PrintedAt { get; set; }
        public int? PrintedBatch { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (PrintedAt is null ? 0 : PrintedAt.GetHashCode());
                hash = (hash * multiplier) ^ (PrintedBatch is null ? 0 : PrintedBatch.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((Printed)obj);
        }

        public bool Equals(Printed other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(Printed other)
        {
            return Equals(PrintedAt, other.PrintedAt)
                && Equals(PrintedBatch, other.PrintedBatch);
        }

        public static bool operator ==(Printed left, Printed right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Printed left, Printed right)
        {
            return !(left == right);
        }
        #endregion
    }
}
