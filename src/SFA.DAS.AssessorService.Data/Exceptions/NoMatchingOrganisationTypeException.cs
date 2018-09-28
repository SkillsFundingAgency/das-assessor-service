using System;

namespace SFA.DAS.AssessorService.Data.Exceptions
{
    public class NoMatchingOrganisationTypeException : Exception
    {
        public NoMatchingOrganisationTypeException(string message) : base(message)
        {
        }
    }
}
