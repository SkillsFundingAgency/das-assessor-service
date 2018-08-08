using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Application.Exceptions
{
    public class AlreadyExistsException: Exception
    {
        public AlreadyExistsException() : base("") { }
        public AlreadyExistsException(string message) : base(message) { }
    }
}
