using System;
using System.Runtime.Serialization;

namespace SFA.DAS.AssessorService.Domain.Exceptions
{
    [Serializable]
    public class NotFound : Exception
    {
        public NotFound()
        {
        }

        public NotFound(string message) : base(message)
        {
        }

        public NotFound(string message, Exception inner) : base(message, inner)
        {
        }

        protected NotFound(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}