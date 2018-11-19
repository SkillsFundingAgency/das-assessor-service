using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.AO
{
    public class DownloadData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<WorksheetDetails> Worksheets { get; set; }
    }
}
