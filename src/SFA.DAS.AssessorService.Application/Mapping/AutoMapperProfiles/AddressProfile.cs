using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;


namespace SFA.DAS.AssessorService.Application.Mapping.AutoMapperProfiles
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<AddressResponse, GetAddressResponse>().ReverseMap();
        }
    }
}
