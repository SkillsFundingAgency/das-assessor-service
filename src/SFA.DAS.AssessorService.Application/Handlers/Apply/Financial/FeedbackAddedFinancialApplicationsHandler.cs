﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Financial;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Financial
{
    public class FeedbackAddedFinancialApplicationsHandler : IRequestHandler<FeedbackAddedFinancialApplicationsRequest, List<FinancialApplicationSummaryItem>>
    {
        private readonly IApplyRepository _repository;

        public FeedbackAddedFinancialApplicationsHandler(IApplyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<FinancialApplicationSummaryItem>> Handle(FeedbackAddedFinancialApplicationsRequest request, CancellationToken cancellationToken)
        {
            return await _repository.GetFeedbackAddedFinancialApplications();
        }
    }
}
