using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

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
            var result = new SetSettingResult();
            if(request.Name.Length == 0 || request.Name.Length > 50)
            {
                result.ValidationMessage = "The name of a setting must be between 1 and 50 characters";
            }

            if (request.Value.Length == 0 || request.Value.Length > 256)
            {
                result.ValidationMessage = "The value of a setting must be between 1 and 256 characters";
            }

            if (string.IsNullOrEmpty(result.ValidationMessage))
            {
                result.SettingResult =
                    await _settingRepository.SetSetting(request.Name, request.Value) ? SettingResult.Created : SettingResult.Updated;
            }
            else
            {
                result.SettingResult = SettingResult.Invalid;
            }

            return result;
        }
    }
}
