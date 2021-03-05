using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.QnA.Api.Types;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Apply
{
    public class SequenceViewModel
    {
        public SequenceViewModel(Sequence sequence, Guid Id, string pageContext, bool allowCancel, List<Section> sections,
            List<ApplySection> applySection, List<ValidationErrorDetail> errorMessages)
        {
            this.Id = Id;
            Sections = sections;
            ApplySections = applySection;
            SequenceNo = sequence.SequenceNo;
            Status = sequence.Status;
            PageContext = pageContext;
            AllowCancel = allowCancel;
            ErrorMessages = errorMessages;
        }

        public string Status { get; set; }
        public string PageContext { get;  }
        public List<Section> Sections { get; }
        public List<ApplySection> ApplySections { get; }
        public Guid Id { get; }
        public int SequenceNo { get; }
        public List<ValidationErrorDetail> ErrorMessages { get; }
        public bool AllowCancel { get; }
    }
}
