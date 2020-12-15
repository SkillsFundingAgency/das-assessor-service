using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class GetEarliestWithdrawalDateHandler : IRequestHandler<GetEarliestWithdrawalDateRequest, DateTime>
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        
        public GetEarliestWithdrawalDateHandler(IDateTimeHelper dateTimeHelper)
        {
            _dateTimeHelper = dateTimeHelper;
        }

        public async Task<DateTime> Handle(GetEarliestWithdrawalDateRequest request, CancellationToken cancellationToken)
        {
            // the earliest withdrawal is currently 6 months from the current date for any organisation or standard
            return await Task.FromResult(_dateTimeHelper.DateTimeNow.AddMonths(6));
        }
    }
}
