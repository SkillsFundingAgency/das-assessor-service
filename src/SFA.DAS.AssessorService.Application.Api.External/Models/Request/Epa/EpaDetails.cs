﻿using SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.Api.External.Models.Request.Epa
{
    public sealed class EpaDetails : IEquatable<EpaDetails>
    {
        [SwaggerRequired]
        public List<EpaRecord> Epas { get; set; }

        #region GetHashCode, Equals and IEquatable
        public override int GetHashCode()
        {
            unchecked
            {
                const int hashBase = (int)2166136261;
                const int multiplier = 16777619;

                int hash = hashBase;
                hash = (hash * multiplier) ^ (Epas is null ? 0 : Epas.GetHashCode());

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return IsEqual((EpaDetails)obj);
        }

        public bool Equals(EpaDetails other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsEqual(other);
        }

        private bool IsEqual(EpaDetails other)
        {
            var epasEqual = Epas is null ? other.Epas is null : Epas.SequenceEqual(other.Epas);

            return epasEqual;
        }

        public static bool operator ==(EpaDetails left, EpaDetails right)
        {
            if (left is null) return right is null;
            return left.Equals(right);
        }

        public static bool operator !=(EpaDetails left, EpaDetails right)
        {
            return !(left == right);
        }
        #endregion
    }
}
