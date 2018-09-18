using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQueryGetDeliveryAreasTests
    {
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected GetDeliveryAreasHandler GetDeliveryAreasHandler;
        protected Mock<ILogger<GetDeliveryAreasHandler>> Logger;
        private List<DeliveryArea> _expectedDeliveryArea;
        private DeliveryArea _deliveryArea1;
        private DeliveryArea _deliveryArea2;

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            _deliveryArea1 = new DeliveryArea { Id = 1, Area = "Area 100", Status = "Live" };
            _deliveryArea2 = new DeliveryArea { Id = 2, Area = "Area 10" };

            Logger = new Mock<ILogger<GetDeliveryAreasHandler>>();

            _expectedDeliveryArea = new List<DeliveryArea>
            {
                _deliveryArea1,
                _deliveryArea2
            };

            RegisterQueryRepository.Setup(r => r.GetDeliveryAreas())
                .Returns(Task.FromResult(_expectedDeliveryArea.AsEnumerable()));

            GetDeliveryAreasHandler = new GetDeliveryAreasHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void GetDeliveryAreasRepoIsCalledWhenHandlerInvoked()
        {
            GetDeliveryAreasHandler.Handle(new GetDeliveryAreasRequest(), new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetDeliveryAreas());
        }

        [Test]
        public void GetDeliveryAreasReturnedExpectedResults()
        {
            var deliveryAreas = GetDeliveryAreasHandler.Handle(new GetDeliveryAreasRequest(), new CancellationToken()).Result;
            deliveryAreas.Count.Should().Be(2);
            deliveryAreas.Should().Contain(_deliveryArea1);
            deliveryAreas.Should().Contain(_deliveryArea2);
        }
    }
}
