using System;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.exceptions
{
    public class NoDataPresentException : Exception
    {
        public NoDataPresentException(string message) : base(message)
        {
        }
    }
}
