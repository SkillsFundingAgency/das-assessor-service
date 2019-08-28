using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class PrivilegeResponse
    {
        public string UserPrivilege { get; set; }
        public string Key { get; set; }
    }
}
