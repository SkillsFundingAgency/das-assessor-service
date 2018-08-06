using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Certificates
{
    public class Certificate : IEquatable<Certificate>
    {
        public CertificateData CertificateData { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public DateTime? PrintedAt { get; set; }
        public int? BatchNumber { get; set; }


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
                hash = (hash * multiplier) ^ CreatedAt.GetHashCode();
                hash = (hash * multiplier) ^ (CreatedBy is null ? 0 : CreatedBy.GetHashCode());
                hash = (hash * multiplier) ^ (UpdatedAt is null ? 0 : UpdatedAt.GetHashCode());
                hash = (hash * multiplier) ^ (UpdatedBy is null ? 0 : UpdatedBy.GetHashCode());
                hash = (hash * multiplier) ^ (DeletedAt is null ? 0 : DeletedAt.GetHashCode());
                hash = (hash * multiplier) ^ (DeletedBy is null ? 0 : DeletedBy.GetHashCode());
                hash = (hash * multiplier) ^ (PrintedAt is null ? 0 : PrintedAt.GetHashCode());
                hash = (hash * multiplier) ^ (BatchNumber is null ? 0 : BatchNumber.GetHashCode());
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
                && string.Equals(Status, other.Status)
                && Equals(CreatedAt, other.CreatedAt)
                && string.Equals(CreatedBy, other.CreatedBy)
                && Equals(UpdatedAt, other.UpdatedAt)
                && string.Equals(UpdatedBy, other.UpdatedBy)
                && Equals(DeletedAt, other.DeletedAt)
                && string.Equals(DeletedBy, other.DeletedBy)
                && Equals(PrintedAt, other.PrintedAt)
                && Equals(BatchNumber, other.BatchNumber);
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
