using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.FrameworkSearch
{
    public class GetFrameworkLearnerHandler : BaseHandler, IRequestHandler<GetFrameworkCertificateQuery, GetFrameworkCertificateResult>
    {
        private readonly IFrameworkLearnerRepository _frameworkLearnerRepository;

        public GetFrameworkLearnerHandler(IMapper mapper, IFrameworkLearnerRepository frameworkLearnerRepository)
            :base(mapper)
        {
            _frameworkLearnerRepository = frameworkLearnerRepository;
        }

        public async Task<GetFrameworkCertificateResult> Handle(GetFrameworkCertificateQuery request, CancellationToken cancellationToken)
        {
            var returnValue =  await _frameworkLearnerRepository.GetById(request.Id);
            return _mapper.Map<GetFrameworkCertificateResult>(returnValue);
        }
    }
}