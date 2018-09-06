using System.Collections.Generic;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GetOptionsRequest: IRequest<List<Option>>
    {
        public int StdCode { get; set; }
    }
}