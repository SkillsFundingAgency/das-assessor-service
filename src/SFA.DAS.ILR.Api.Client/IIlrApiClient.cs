using System;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.ILR.Api.Client
{
    public interface IIlrApiClient
    {
        SearchResultViewModel Search(SearchQueryViewModel vm);
    }

    public class IlrApiClient : IIlrApiClient
    {
        public SearchResultViewModel Search(SearchQueryViewModel vm)
        {
            throw new NotImplementedException();
        }
    }
}
