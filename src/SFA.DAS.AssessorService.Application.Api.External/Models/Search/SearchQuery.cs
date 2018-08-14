using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Search
{
    public class SearchQuery : IEquatable<SearchQuery>
    {
        public long Uln { get; set; }
        public string Surname { get; set; }
        public int UkPrn { get; set; }
        public string Username { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ Uln.GetHashCode();
                hash = (hash * multiplier) ^ (Surname is null ? 0 : Surname.GetHashCode());
                hash = (hash * multiplier) ^ UkPrn.GetHashCode();
                hash = (hash * multiplier) ^ (Username is null ? 0 : Username.GetHashCode());
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((SearchQuery)obj);
        }

        public bool Equals(SearchQuery other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(SearchQuery other)
        {
            return Equals(Uln, other.Uln)
                && string.Equals(Surname, other.Surname)
                && Equals(UkPrn, other.UkPrn)
                && string.Equals(Username, other.Username);
        }

        public static bool operator ==(SearchQuery left, SearchQuery right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SearchQuery left, SearchQuery right)
        {
            return !(left == right);
        }
        #endregion
    }
}
