using System;

namespace SFA.DAS.AssessorService.Application.Exceptions
{
    public sealed class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException() : base("") { }
        public ResourceNotFoundException(string message) : base(message) { }
    }
}
