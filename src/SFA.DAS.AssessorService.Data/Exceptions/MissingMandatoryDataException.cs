using System;

namespace SFA.DAS.AssessorService.Data.Exceptions
{
    public class MissingMandatoryDataException : Exception
    {
        public MissingMandatoryDataException(string message) : base(message)
        {
        }
    }
}
