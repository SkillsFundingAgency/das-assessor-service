using System;

namespace SFA.DAS.AssessmentOrgs.Api.Client.Core
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}