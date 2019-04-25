using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class ContactBoolResponse
    {
        public ContactBoolResponse(bool result)
        {
            Result = result;
        }
        public bool Result { get; set; }
    }
}
