using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class GetSettingHandler : IRequestHandler<GetSettingRequest, string>
    {
        private readonly ISettingRepository _settingRepository;

        public GetSettingHandler(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }
        public async Task<string> Handle(GetSettingRequest request, CancellationToken cancellationToken)
        {
            return await _settingRepository.GetSetting(request.Name);
        }
    }
}
