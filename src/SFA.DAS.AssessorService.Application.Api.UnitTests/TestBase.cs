using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests
{
    public class TestBase
    {
        protected IMapper Mapper;

        public TestBase()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<CreateOrganisationRequest, Organisation>();
                cfg.CreateMap<Organisation, OrganisationResponse>();
                cfg.CreateMap<Contact, CreateContactRequest>();
                cfg.CreateMap<Contact, ContactResponse>();

            });

            var serviceProvider = services.BuildServiceProvider();
            Mapper = serviceProvider.GetRequiredService<IMapper>();
        }
    }
}
