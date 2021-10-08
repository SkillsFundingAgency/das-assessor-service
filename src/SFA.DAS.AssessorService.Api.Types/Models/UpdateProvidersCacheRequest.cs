using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public enum ProvidersCacheUpdateType
    {
        UpdateProvidersFromLearners = 1,
        UpdateExistingProviders = 2
    }

    public class UpdateProvidersCacheRequest : IRequest
    {
        public ProvidersCacheUpdateType UpdateType { get; set; }
    }
}
