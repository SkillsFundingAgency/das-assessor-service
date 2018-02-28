namespace SFA.DAS.AssessorService.Api.Types.Models
{
    using System;
    using MediatR;

    public class DeleteContactRequest : IRequest
    {
        public Guid Id { get; set; }                  
    }
}
