using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Standards
{
    public class SetSettingHandler : IRequestHandler<SetSettingRequest, SetSettingResult>
    {
        private readonly ISettingRepository _settingRepository;

        public SetSettingHandler(ISettingRepository settingRepository)
        {
            _settingRepository = settingRepository;
        }

        public async Task<SetSettingResult> Handle(SetSettingRequest request, CancellationToken cancellationToken)
        {
            var settingExists = (await _settingRepository.GetSetting(request.Name) != null);
            if (settingExists)
            {
                await _settingRepository.UpdateSetting(request.Name, request.Value);
                return SetSettingResult.Updated;
            }

            await _settingRepository.CreateSetting(request.Name, request.Value);
            return SetSettingResult.Created;
        }
    }
}
