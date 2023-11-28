using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Settings
{
    public interface IManagedIdentityApiAuthentication
    {
        string Identifier { get; set; }
        string ApiBaseAddress { get; set; }
    }
}
