using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Response.Epa
{
    public sealed class EpaRecord : IEquatable<EpaRecord>
    {
        public DateTime EpaDate { get; set; }
        public string EpaOutcome { get; set; }
        public bool? Resit { get; set; }
        public bool? Retake { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ EpaDate.GetHashCode();
                hash = (hash * multiplier) ^ (EpaOutcome is null ? 0 : EpaOutcome.GetHashCode());
                hash = (hash * multiplier) ^ (Resit is null ? 0 : Resit.GetHashCode());
                hash = (hash * multiplier) ^ (Retake is null ? 0 : Retake.GetHashCode());

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((EpaRecord)obj);
        }

        public bool Equals(EpaRecord other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(EpaRecord other)
        {
            return Equals(EpaDate, other.EpaDate)
                && string.Equals(EpaOutcome, other.EpaOutcome)
                && Equals(Resit, other.Resit)
                && Equals(Retake, other.Retake);
        }

        public static bool operator ==(EpaRecord left, EpaRecord right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(EpaRecord left, EpaRecord right)
        {
            return !(left == right);
        }
        #endregion
    }
}
