using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.AssessmentOrgsImport.exceptions
{
    public class NoMatchingOrganisationTypeException : Exception
    {
        public NoMatchingOrganisationTypeException(string message) : base(message)
        {
        }
    }
}
