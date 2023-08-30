using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Domain.Validation
{
    public static class Guard
    {
        public static void NotNullOrWhiteSpace(string str, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("This string cannot be null or all-whitespace.", parameterName);
            }
        }
    }
}
