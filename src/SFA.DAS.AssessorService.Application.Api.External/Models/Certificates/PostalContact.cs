using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Certificates
{
    public class PostalContact : IEquatable<PostalContact>
    {
        public string ContactName { get; set; }
        public string Department { get; set; }
        public string Organisation { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string PostCode { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (ContactName is null ? 0 : ContactName.GetHashCode());
                hash = (hash * multiplier) ^ (Department is null ? 0 : Department.GetHashCode());
                hash = (hash * multiplier) ^ (Organisation is null ? 0 : Organisation.GetHashCode());
                hash = (hash * multiplier) ^ (AddressLine1 is null ? 0 : AddressLine1.GetHashCode());
                hash = (hash * multiplier) ^ (AddressLine2 is null ? 0 : AddressLine2.GetHashCode());
                hash = (hash * multiplier) ^ (AddressLine3 is null ? 0 : AddressLine3.GetHashCode());
                hash = (hash * multiplier) ^ (AddressLine4 is null ? 0 : AddressLine4.GetHashCode());
                hash = (hash * multiplier) ^ (PostCode is null ? 0 : PostCode.GetHashCode());

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((PostalContact)obj);
        }

        public bool Equals(PostalContact other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(PostalContact other)
        {
            return string.Equals(ContactName, other.ContactName)
                && string.Equals(Department, other.Department)
                && string.Equals(Organisation, other.Organisation)
                && string.Equals(AddressLine1, other.AddressLine1)
                && string.Equals(AddressLine2, other.AddressLine2)
                && string.Equals(AddressLine3, other.AddressLine3)
                && string.Equals(AddressLine4, other.AddressLine4)
                && string.Equals(PostCode, other.PostCode);
        }

        public static bool operator ==(PostalContact left, PostalContact right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(PostalContact left, PostalContact right)
        {
            return !(left == right);
        }
        #endregion
    }
}
