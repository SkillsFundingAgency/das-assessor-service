using System;

namespace SFA.DAS.AssessorService.Application.Api.Client.Exceptions
{
    public class EntityAlreadyExistsException : ApplicationException
    {
        public EntityAlreadyExistsException()
        {
            
        }

        public EntityAlreadyExistsException(string message) : base(message)
        {

        }

        public EntityAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}