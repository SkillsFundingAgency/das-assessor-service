using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class GetDeliveryAreasByOrganisationStandardIdTests: TestBase
    {
        private OrganisationModel _organisation1;
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterQueryRepository _repository;
        private string _organisationId1;
        private OrganisationStandardModel _organisationStandard;
        private string _organisationIdCreated;
        private readonly int _standardCode = 5;
        private readonly int _orgStandardId = 50;
        private DeliveryAreaModel _deliveryArea1;
        private DeliveryAreaModel _deliveryArea2;
        private readonly int _deliveryAreaId1 = 1;
        private readonly int _deliveryAreaId2 = 2;
        
        [OneTimeSetUp]
        public void SetupOrganisationTests()
        {
            var databaseConnection = new SqlConnection(_databaseService.SqlConnectionStringTest);
            var unitOfWork = new UnitOfWork(databaseConnection);

            _repository = new RegisterQueryRepository(unitOfWork);
            
            _organisationIdCreated = "EPA0987";
            _organisationId1 = "EPA0987"; 
            _organisation1 = new OrganisationModel
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.AddYears(-1).Date,
                DeletedAt = null,
                EndPointAssessorName = "Name 1",
                EndPointAssessorOrganisationId = _organisationId1,
                EndPointAssessorUkprn = 876544,
                PrimaryContact = null,
                Status = "new",
                UpdatedAt = null,
                OrganisationTypeId = null,
                OrganisationData = null
            };

            _organisationStandard = new OrganisationStandardModel
            {
                Id = _orgStandardId,
                EndPointAssessorOrganisationId = _organisationIdCreated,
                StandardCode = _standardCode,
                Status = OrganisationStatus.New,
                EffectiveFrom = DateTime.Today.AddDays(-7),
                EffectiveTo = DateTime.Today.AddDays(10),
                DateStandardApprovedOnRegister = DateTime.Today.AddDays(-50),
                Comments = "comments go here"
            };        

            _deliveryArea1 = new DeliveryAreaModel { Id = 10, Status = "Live", Area = "North West" };
            _deliveryArea2 = new DeliveryAreaModel { Id = 20, Status = "New", Area = "Some Other" };
            var deliveryAreas = new List<DeliveryAreaModel> { _deliveryArea1, _deliveryArea2 };

            DeliveryAreaHandler.InsertRecords(deliveryAreas);
            
            var deliveryArea1= new OrganisationStandardDeliveryAreaModel {
                Id = _deliveryAreaId1,
                Comments = "comments 1",
                DeliveryAreaId = _deliveryArea1.Id,
                OrganisationStandardId = _orgStandardId,
                Status = OrganisationStatus.New,  
            };
            
            var deliveryArea2= new OrganisationStandardDeliveryAreaModel {
                Id = _deliveryAreaId2,
                Comments = "comments 1",
                DeliveryAreaId = _deliveryArea2.Id,
                OrganisationStandardId = _orgStandardId,
                Status = OrganisationStatus.New
            };
            
            OrganisationHandler.InsertRecords(new List<OrganisationModel> { _organisation1});
            OrganisationStandardHandler.InsertRecord(_organisationStandard);
            OrganisationStandardDeliveryAreaHandler.InsertRecords(new List<OrganisationStandardDeliveryAreaModel>{
                deliveryArea1,deliveryArea2
            });
            
        }

        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationStandardDeliveryAreaHandler.DeleteRecords(new List<int>{1,2});
            OrganisationStandardHandler.DeleteRecord(_orgStandardId);
            OrganisationHandler.DeleteRecords(new List<Guid> { _organisation1.Id });
        }
    }
    }