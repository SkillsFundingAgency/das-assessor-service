using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class GenerateBatchNumberRequestHandler : IRequestHandler<GenerateBatchNumberRequest, int>
    {
        private readonly ICertificateRepository _certificateRepository;

        public GenerateBatchNumberRequestHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task<int> Handle(GenerateBatchNumberRequest request, CancellationToken cancellationToken)
        {
            return await _certificateRepository.GenerateBatchNumber();
        }
    }
}