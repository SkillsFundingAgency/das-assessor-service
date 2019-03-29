using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.ContactHandlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Contacts.CreateContactHandlerTests
{
    [TestFixture]
    public class CreateContactsHandlerTestBase
    {
        protected Mock<IContactRepository> ContactRepository;
        protected Mock<IContactQueryRepository> ContactQueryRepository;
        protected Mock<IDfeSignInService> SignInService;
        protected Mock<IMediator> Mediator;
        protected CreateContactHandler CreateContactHandler;

        [SetUp]
        public void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CreateContactRequest, Contact>().ReverseMap();
            });
            
            ContactRepository = new Mock<IContactRepository>();
            ContactQueryRepository = new Mock<IContactQueryRepository>();
            SignInService = new Mock<IDfeSignInService>();
            Mediator = new Mock<IMediator>();
            CreateContactHandler = new CreateContactHandler(ContactRepository.Object, ContactQueryRepository.Object, SignInService.Object, Mediator.Object);
        }
    }
}