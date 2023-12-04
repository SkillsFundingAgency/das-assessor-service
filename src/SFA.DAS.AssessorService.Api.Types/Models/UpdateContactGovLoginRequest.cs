using System;
using MediatR;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models;

public class UpdateContactGovLoginRequest : IRequest<UpdateContactGovLoginResponse>
{
    public string GovIdentifier { get; set; }
    public Guid SignInId { get; set; }
    public Guid ContactId { get; set; }
}

public class UpdateContactGovLoginResponse
{
    public Contact Contact { get; set; }
}