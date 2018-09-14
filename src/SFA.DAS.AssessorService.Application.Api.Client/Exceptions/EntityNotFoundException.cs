using System;

namespace SFA.DAS.AssessorService.Application.Api.Client.Exceptions
{
    public class EntityNotFoundException : ApplicationException
    {
        public EntityNotFoundException()
        {
            
        }

        public EntityNotFoundException(string message) : base(message)
        {

        }
        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}