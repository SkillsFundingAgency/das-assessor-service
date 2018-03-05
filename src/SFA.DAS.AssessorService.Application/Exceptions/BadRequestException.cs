using System;

namespace SFA.DAS.AssessorService.Application.Exceptions
{
    public sealed class BadRequestException : Exception
    {
        public BadRequestException() : base("") { }
        public BadRequestException(string message) : base(message) { }
    }
}
