﻿using MediatR;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetAparSummaryLastUpdatedRequest : IRequest<DateTime>
    {
    }
}
