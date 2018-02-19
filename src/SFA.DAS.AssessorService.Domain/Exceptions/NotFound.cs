namespace SFA.DAS.AssessorService.Domain.Exceptions
{
    using System;

    [Serializable]
    public class NotFound : Exception
    {
        public NotFound() { }
        public NotFound(string message) : base(message) { }
        public NotFound(string message, Exception inner) : base(message, inner) { }
        protected NotFound(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
