﻿using System;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Certificates
{
    public class Standard : IEquatable<Standard>
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public DateTime PublicationDate { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ Code.GetHashCode();
                hash = (hash * multiplier) ^ (Name is null ? 0 : Name.GetHashCode());
                hash = (hash * multiplier) ^ Level.GetHashCode();
                hash = (hash * multiplier) ^ PublicationDate.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((Standard)obj);
        }

        public bool Equals(Standard other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(Standard other)
        {
            return Equals(Code, other.Code)
                && string.Equals(Name, other.Name)
                && Equals(Level, other.Level)
                && Equals(PublicationDate, other.PublicationDate);
        }

        public static bool operator ==(Standard left, Standard right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(Standard left, Standard right)
        {
            return !(left == right);
        }
        #endregion
    }
}
