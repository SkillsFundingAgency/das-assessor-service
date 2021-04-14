using SFA.DAS.AssessorService.Web.ViewModels.Shared;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    public class CertificateSession
    {
        public Guid CertificateId { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string EPAOName { get; set; }
        public string StandardUId { get; set; }
        public List<string> Options { get; set; }
        public List<StandardVersion> Versions { get; set; }
    }
}