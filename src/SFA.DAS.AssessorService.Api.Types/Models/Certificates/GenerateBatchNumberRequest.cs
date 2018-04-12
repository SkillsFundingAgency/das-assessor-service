using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class GenerateBatchNumberRequest : IRequest<int>
    {
        public GenerateBatchNumberRequest()
        {
           
        }
    }
}