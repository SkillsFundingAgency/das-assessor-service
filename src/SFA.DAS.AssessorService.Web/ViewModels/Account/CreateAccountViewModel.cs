using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.ViewModels.Account
{
    public class CreateAccountViewModel
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"{GivenName} {FamilyName}, {Email}";
        }
    }
}
