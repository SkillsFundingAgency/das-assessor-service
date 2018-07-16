using System;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.exceptions
{
    public class MissingMandatoryDataException : Exception
    {
        public MissingMandatoryDataException(string message) : base(message)
        {
        }
    }
}
