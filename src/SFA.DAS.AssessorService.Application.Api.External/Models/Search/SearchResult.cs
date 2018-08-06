using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Search
{
    public class SearchResult : IEquatable<SearchResult>
    {
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int StdCode { get; set; }
        public string Standard { get; set; }
        public int Level { get; set; }
        public int FundingModel { get; set; }
        public int UkPrn { get; set; }
        public string Option { get; set; }
        public DateTime? LearnStartDate { get; set; }
        public string OverallGrade { get; set; }
        public DateTime? AchDate { get; set; }
        public Guid CertificateId { get; set; }
        public string CertificateReference { get; set; }
        public string CertificateStatus { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool ShowExtraInfo { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ Uln.GetHashCode();
                hash = (hash * multiplier) ^ (GivenNames is null ? 0 : GivenNames.GetHashCode());
                hash = (hash * multiplier) ^ (FamilyName is null ? 0 : FamilyName.GetHashCode());
                hash = (hash * multiplier) ^ DateOfBirth.GetHashCode();
                hash = (hash * multiplier) ^ StdCode.GetHashCode();
                hash = (hash * multiplier) ^ (Standard is null ? 0 : Standard.GetHashCode());
                hash = (hash * multiplier) ^ Level.GetHashCode();
                hash = (hash * multiplier) ^ FundingModel.GetHashCode();
                hash = (hash * multiplier) ^ UkPrn.GetHashCode();
                hash = (hash * multiplier) ^ (Option is null ? 0 : Option.GetHashCode());
                hash = (hash * multiplier) ^ (LearnStartDate is null ? 0 : LearnStartDate.GetHashCode());
                hash = (hash * multiplier) ^ (OverallGrade is null ? 0 : OverallGrade.GetHashCode());
                hash = (hash * multiplier) ^ (AchDate is null ? 0 : AchDate.GetHashCode());
                hash = (hash * multiplier) ^ CertificateId.GetHashCode();
                hash = (hash * multiplier) ^ (CertificateReference is null ? 0 : CertificateReference.GetHashCode());
                hash = (hash * multiplier) ^ (CertificateStatus is null ? 0 : CertificateStatus.GetHashCode());
                hash = (hash * multiplier) ^ (SubmittedAt is null ? 0 : SubmittedAt.GetHashCode());
                hash = (hash * multiplier) ^ (SubmittedBy is null ? 0 : SubmittedBy.GetHashCode());
                hash = (hash * multiplier) ^ (UpdatedAt is null ? 0 : UpdatedAt.GetHashCode());
                hash = (hash * multiplier) ^ (UpdatedBy is null ? 0 : UpdatedBy.GetHashCode());
                hash = (hash * multiplier) ^ ShowExtraInfo.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((SearchResult)obj);
        }

        public bool Equals(SearchResult other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(SearchResult other)
        {
            return Equals(Uln, other.Uln)
                && string.Equals(GivenNames, other.GivenNames)
                && string.Equals(FamilyName, other.FamilyName)
                && Equals(DateOfBirth, other.DateOfBirth)
                && Equals(StdCode, other.StdCode)
                && string.Equals(Standard, other.Standard)
                && Equals(Level, other.Level)
                && Equals(FundingModel, other.FundingModel)
                && Equals(UkPrn, other.UkPrn)
                && string.Equals(Option, other.Option)
                && Equals(LearnStartDate, other.LearnStartDate)
                && string.Equals(OverallGrade, other.OverallGrade)
                && Equals(AchDate, other.AchDate)
                && Equals(CertificateId, other.CertificateId)
                && string.Equals(CertificateReference, other.CertificateReference)
                && string.Equals(CertificateStatus, other.CertificateStatus)
                && Equals(SubmittedAt, other.SubmittedAt)
                && string.Equals(SubmittedBy, other.SubmittedBy)
                && Equals(UpdatedAt, other.UpdatedAt)
                && string.Equals(UpdatedBy, other.UpdatedBy)
                && Equals(ShowExtraInfo, other.ShowExtraInfo);
        }

        public static bool operator ==(SearchResult left, SearchResult right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(SearchResult left, SearchResult right)
        {
            return !(left == right);
        }
        #endregion
    }
}
