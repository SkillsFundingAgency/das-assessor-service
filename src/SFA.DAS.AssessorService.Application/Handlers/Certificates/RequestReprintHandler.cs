using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using NotFound = SFA.DAS.AssessorService.Domain.Exceptions.NotFound;

namespace SFA.DAS.AssessorService.Application.Handlers.Certificates
{
    public class RequestReprintHandler : IRequestHandler<CertificateReprintRequest>
    {
        private readonly ICertificateRepository _certificateRepository;

        public RequestReprintHandler(ICertificateRepository certificateRepository)
        {
            _certificateRepository = certificateRepository;
        }

        public async Task Handle(CertificateReprintRequest request, CancellationToken cancellationToken)
        {
            var result = await _certificateRepository.RequestReprint(request.Username, request.CertificateReference, request.LastName, request.AchievementDate);
            if (result == false)
                throw new NotFound();
        }
    }
}