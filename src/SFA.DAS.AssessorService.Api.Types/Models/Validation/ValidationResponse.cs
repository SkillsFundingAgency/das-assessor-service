using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.Validation
{
    public class ValidationResponse
    {
        public ValidationResponse()
        {
            if (Errors == null) { Errors = new List<ValidationErrorDetail>(); }
        }

        public List<ValidationErrorDetail> Errors { get; set; }
        public bool IsValid => Errors.Count == 0;
    }
}
