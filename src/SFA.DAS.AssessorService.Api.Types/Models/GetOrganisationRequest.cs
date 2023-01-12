using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetOrganisationRequest : IRequest<Organisation>
    {
        public Guid Id { get; set; }
    }
}