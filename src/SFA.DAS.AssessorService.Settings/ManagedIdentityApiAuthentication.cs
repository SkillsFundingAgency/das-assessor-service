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
         public string IdentifierUri { get; set; }

         public string ApiBaseUrl { get; set; }
    }
}
