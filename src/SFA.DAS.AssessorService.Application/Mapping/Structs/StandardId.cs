using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Application.Mapping.Structs
{
    public struct StandardId
    {
        public string StandardUId { get; }
        public string IFateReferenceNumber { get; }
        public int LarsCode { get; }
        public StandardIdType IdType { get; }

        public StandardId(string id)
        {
            StandardUId = null;
            IFateReferenceNumber = null;
            LarsCode = -1;

            if (int.TryParse(id, out var larsCode)) // Lars Code
            {
                LarsCode = larsCode;
                IdType = StandardIdType.LarsCode;
            }
            else if (id.Length == 6) // Ifate Ref
            {
                IdType = StandardIdType.IFateReferenceNumber;
                IFateReferenceNumber = id;
            }
            else // Assume StandardUId
            {
                IdType = StandardIdType.StandardUId;
                StandardUId = id;
            }
        }

        public enum StandardIdType
        {
            StandardUId,
            LarsCode,
            IFateReferenceNumber
        }
    }
}
