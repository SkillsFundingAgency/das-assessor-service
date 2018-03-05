using System;

namespace SFA.DAS.AssessorService.Application.Exceptions
{
    public sealed class UnauthorisedException : Exception
    {
        public UnauthorisedException() : base("") { }
        public UnauthorisedException(string message) : base(message) { }
    }
}
