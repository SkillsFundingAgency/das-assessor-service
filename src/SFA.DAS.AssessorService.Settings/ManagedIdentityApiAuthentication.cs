using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Settings
{
    public class ManagedIdentityApiAuthentication : IManagedIdentityApiAuthentication
    {
        [JsonRequired] public string Identifier { get; set; }

        [JsonRequired] public string ApiBaseAddress { get; set; }
    }
}
