using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GovUkIdentifierWrapper
    {
        public GovUkIdentifierWrapper(string govUkIdentifier)
        {
            GovUkIdentifier = govUkIdentifier;
        }

        public string GovUkIdentifier { get; set; }
        
    }
}
