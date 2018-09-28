using System;

namespace SFA.DAS.AssessorService.Data.Exceptions
{
    public class NoDataPresentException : Exception
    {
        public NoDataPresentException(string message) : base(message)
        {
        }
    }
}
