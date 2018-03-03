using System;

namespace SFA.DAS.AssessorService.Application.Exceptions
{
    public sealed class ValidationException : Exception
    {
        public ValidationException() : base("") { }
        public ValidationException(string message) : base(message) { }
    }
}
