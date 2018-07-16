using System;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.exceptions
{
    public class WorksheetNotAvailableException : Exception
    {
        public WorksheetNotAvailableException(string message) : base(message)
        {
        }
    }
}
