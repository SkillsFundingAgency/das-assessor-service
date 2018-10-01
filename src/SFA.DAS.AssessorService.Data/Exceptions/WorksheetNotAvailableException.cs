using System;

namespace SFA.DAS.AssessorService.Data.Exceptions
{
    public class WorksheetNotAvailableException : Exception
    {
        public WorksheetNotAvailableException(string message) : base(message)
        {
        }
    }
}
