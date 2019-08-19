using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class SequenceViewModel
    {
        public SequenceViewModel(Sequence sequence, Guid applicationId, List<Section> sections, List<ValidationErrorDetail> errorMessages)
        {
            ApplicationId = applicationId;
            Sections = sections;
            SequenceNo = (int)sequence.SequenceNo;
            Status = sequence.Status;
            ErrorMessages = errorMessages;
        }

        public string Status { get; }
        public List<Section> Sections { get; }

        public Guid ApplicationId { get; }
        public int SequenceNo { get; }
        public List<ValidationErrorDetail> ErrorMessages { get; }
    }
}
