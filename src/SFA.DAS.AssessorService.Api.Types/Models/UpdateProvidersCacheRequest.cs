using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public enum ProvidersCacheUpdateType
    {
        InsertProvidersFromApprovalsExtract = 1,
        RefreshExistingProviders = 2
    }

    public class UpdateProvidersCacheRequest : IRequest
    {
        public ProvidersCacheUpdateType UpdateType { get; set; }
    }
}
