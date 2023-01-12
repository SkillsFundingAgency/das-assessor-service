using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Addresses;
using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Addresses
{
    [TestFixture]
    public class WhenHandlingGetAddressesRequest
    {
        private Mock<ILogger<GetAddressesHandler>> _loggerMock;
        private Mock<IOuterApiService> _outerApiServiceMock;
        private GetAddressesHandler _sut;


        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<GetAddressesHandler>>();
            _outerApiServiceMock = new Mock<IOuterApiService>();

            _outerApiServiceMock.Setup(m => m.GetAddresses(It.IsAny<string>())).ReturnsAsync(new GetAddressesResponse()
            {
                Addresses = new List<GetAddressResponse>
                {
                    new GetAddressResponse {Uprn = "1"},
                    new GetAddressResponse {Uprn = "2"}
                }
            });

            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AddressResponse, GetAddressResponse>();
            });

            _sut = new GetAddressesHandler(_loggerMock.Object, _outerApiServiceMock.Object);
        }

        [Test]
        public async Task Then_Get_Addresses_From_AssessorsOuterApi()
        {
            // Arrange.


            // Act.
            await _sut.Handle(new AssessorService.Api.Types.Models.GetAddressesRequest(), new CancellationToken());

            // Assert.
            _outerApiServiceMock.Verify(m => m.GetAddresses(It.IsAny<string>()));
        }
    }
}