
namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class GetStandardByIdRequest : IGetApiRequest
    {
        /// <summary>
        /// Id can be Lars Code, Ifate Standard Reference, or StandardUId
        /// </summary>
        public string Id { get; }
        public GetStandardByIdRequest(string id)
        {
            Id = id;
        }

        public string GetUrl => $"trainingcourses/{Id}";
    }
}
