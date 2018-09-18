using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class DeliveryAreasTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterQueryRepository _repository;
        private DeliveryAreaModel _deliveryArea1;
        private DeliveryAreaModel _deliveryArea2;

        [OneTimeSetUp]
        public void SetupDeliveryAreasTests()
        {
            _repository = new RegisterQueryRepository(_databaseService.WebConfiguration);
            _deliveryArea1 = new DeliveryAreaModel { Id = 10, Status = "Live", Area = "North West" };
            _deliveryArea2 = new DeliveryAreaModel { Id = 20, Status = "New", Area = "Some Other" };
            var deliveryAreas = new List<DeliveryAreaModel> { _deliveryArea1, _deliveryArea2 };

            DeliveryAreaHandler.InsertRecords(deliveryAreas);
        }

        [Test]
        public void RunGetAllOrganisationTypesAndCheckAllOrganisationsExpectedAreReturned()
        {

            var deliveryAreasReturned = _repository.GetDeliveryAreas().Result.ToArray();

            Assert.AreEqual(2, deliveryAreasReturned.Count(), $@"Expected 2 delivery Areas back but got {deliveryAreasReturned.Count()}");
            Assert.AreEqual(1, deliveryAreasReturned.Count(x => x.Id == _deliveryArea1.Id), "Delivery Area 1 Id was not found");
            Assert.AreEqual(1, deliveryAreasReturned.Count(x => x.Id == _deliveryArea2.Id), "Delivery Area 2 Id was not found");
        }

        [OneTimeTearDown]
        public void TearDownDeliveryAreasTests()
        {
            DeliveryAreaHandler.DeleteRecords(new List<int> { _deliveryArea1.Id, _deliveryArea2.Id });
        }
    }
}
