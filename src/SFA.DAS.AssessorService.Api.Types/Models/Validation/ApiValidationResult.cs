using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Validation
{
    public class ApiValidationResult
    {
        public bool IsValid { get; set; }
        public List<KeyValuePair<string, string>> ErrorMessages { get; set; }

        public ApiValidationResult()
        {
            ErrorMessages = new List<KeyValuePair<string, string>>();
        }
    }
}