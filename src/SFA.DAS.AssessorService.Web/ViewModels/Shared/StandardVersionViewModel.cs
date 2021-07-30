using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Shared
{
    public class StandardVersionViewModel
    {
        public string Title { get; set; }
        public string StandardUId { get; set; }
        public string Version { get; set; }
        /// <summary>
        /// NB This isn't always populated on the return Standard Version Api Type
        /// It depends on the Api Call being performed and will be addressed as part of the 
        /// tech debt ticket that looks at caching options values.
        /// SV-595
        /// </summary>
        public IEnumerable<string> Options { get; set; }
    }
}
