using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Certificates
{
    public class Certificate : IEquatable<Certificate>
    {
        public CertificateData CertificateData { get; set; }

        public Status Status { get; set; }

        public Created Created { get; set; }

        public Submitted Submitted { get; set; }

        public Printed Printed { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (CertificateData is null ? 0 : CertificateData.GetHashCode());
                hash = (hash * multiplier) ^ (Status is null ? 0 : Status.GetHashCode());
                hash = (hash * multiplier) ^ (Created is null ? 0 : Created.GetHashCode());
                hash = (hash * multiplier) ^ (Submitted is null ? 0 : Submitted.GetHashCode());
                hash = (hash * multiplier) ^ (Printed is null ? 0 : Printed.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((Certificate)obj);
        }

        public bool Equals(Certificate other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(Certificate other)
        {
            return Equals(CertificateData, other.CertificateData)
                && Equals(Status, other.Status)
                && Equals(Created, other.Created)
                && Equals(Submitted, other.Submitted)
                && Equals(Printed, other.Printed);
        }

        public static bool operator ==(Certificate left, Certificate right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Certificate left, Certificate right)
        {
            return !(left == right);
        }
        #endregion
    }
}
