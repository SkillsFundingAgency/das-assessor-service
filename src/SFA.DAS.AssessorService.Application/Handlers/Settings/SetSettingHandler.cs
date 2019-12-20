using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class SetSettingHandler : IRequestHandler<SetSettingRequest>
    {
        private readonly ISettingRepository _settingRepository;

        public SetSettingHandler(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task Handle(SetSettingRequest request, CancellationToken cancellationToken)
        {
            await _settingRepository.SetSetting(request.Name, request.Value);
        }
    }
}
