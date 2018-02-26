using System;

namespace SFA.DAS.AssessorService.ExternalApis
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}