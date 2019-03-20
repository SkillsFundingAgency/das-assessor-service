using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Certificates
{
    public class SubmitCertificate : IEquatable<SubmitCertificate>
    {
        public string RequestId { get; set; }
        [Required]
        public long Uln { get; set; }
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        [Required]
        public string FamilyName { get; set; }
        [Required]
        public string CertificateReference { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ Uln.GetHashCode();
                hash = (hash * multiplier) ^ StandardCode.GetHashCode();
                hash = (hash * multiplier) ^ (StandardReference is null ? 0 : StandardReference.GetHashCode());
                hash = (hash * multiplier) ^ (FamilyName is null ? 0 : FamilyName.GetHashCode());
                hash = (hash * multiplier) ^ (CertificateReference is null ? 0 : CertificateReference.GetHashCode());

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((SubmitCertificate)obj);
        }

        public bool Equals(SubmitCertificate other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(SubmitCertificate other)
        {
            return Equals(Uln, other.Uln)
                && Equals(StandardCode, other.StandardCode)
                && string.Equals(StandardReference, other.StandardReference)
                && string.Equals(FamilyName, other.FamilyName)
                && string.Equals(CertificateReference, other.CertificateReference);
        }

        public static bool operator ==(SubmitCertificate left, SubmitCertificate right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(SubmitCertificate left, SubmitCertificate right)
        {
            return !(left == right);
        }
        #endregion
    }
}
