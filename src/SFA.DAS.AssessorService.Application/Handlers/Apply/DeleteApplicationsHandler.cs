using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class DeleteApplicationsHandler : IRequestHandler<DeleteApplicationsRequest, bool>
    {
        private readonly IApplyRepository _applyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteApplicationsHandler> _logger;

        public DeleteApplicationsHandler(IApplyRepository applyRepository,
            IUnitOfWork unitOfWork, ILogger<DeleteApplicationsHandler> logger)
        {
            _applyRepository = applyRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteApplicationsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                string deletedBy = "System";

                _unitOfWork.Begin();

                if (request.DeletingContactId.HasValue)
                    deletedBy = request.DeletingContactId.ToString();

                foreach (var id in request.ApplicationIds)
                    await _applyRepository.DeleteApplication(id, deletedBy);

                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete applications {string.Join(",", request.ApplicationIds.Select(x => x.ToString()))}");
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}