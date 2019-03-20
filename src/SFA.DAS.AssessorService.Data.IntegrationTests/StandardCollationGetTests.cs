using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    [TestFixture]
    public class StandardCollationGetTests : TestBase
    {
        private StandardCollationModel _standardCollation1;
        private StandardCollationModel _standardCollation2;
        private StandardCollationModel _standardCollation3;
        private readonly DatabaseService _databaseService = new DatabaseService();
        private StandardRepository _repository;
        private int _standardId1;
        private int _standardId2;
        private int _standardId3;
        private string _referenceNumber1;
        private string _referenceNumber2;
        private string _title1;
        private string _title2;
        private string _title3;
        private string _standardData1;
        private int _standardDataLevel1;

        [OneTimeSetUp]
        public void SetupOrganisationTests()
        {
            _repository = new StandardRepository(_databaseService.WebConfiguration, null,null);
            _standardId1 = 1;
            _standardId2 = 10;
            _standardId3 = 100;
            _referenceNumber1 = "ST0001";
            _referenceNumber2 = "ST0010";
            _title1 = "Standard 1";
            _title2 = "Standard title 2";
            _title3 = "standard title 3";
            _standardDataLevel1 = 4;
            _standardData1 = "{\"Level\": "+ _standardDataLevel1 + " }";

            _standardCollation1 = new StandardCollationModel
            {
                StandardId = _standardId1,
                ReferenceNumber = _referenceNumber1,
                Title=_title1,
                StandardData = _standardData1
            };

            _standardCollation2 = new StandardCollationModel
            {
                StandardId = _standardId2,
                ReferenceNumber = _referenceNumber2,
                Title = _title2,
                StandardData = null
            };

            _standardCollation3 = new StandardCollationModel
            {
                StandardId = _standardId3,
                ReferenceNumber = null,
                Title = _title3,
                StandardData = null
            };

            StandardCollationHandler.InsertRecords(new List<StandardCollationModel> {_standardCollation1, _standardCollation2, _standardCollation3});
        }

        [Test]
        public void RunGetAllStandardsAndCheckAllStandardsExpectedAreReturned()
        {
            var standardsReturned = _repository.GetStandardCollations().Result.ToList();
            Assert.AreEqual(3, standardsReturned.Count, $@"Expected 3 standards back but got {standardsReturned.Count}");
            Assert.AreEqual(1, standardsReturned.Count(x => x.StandardId == _standardCollation1.StandardId), "Standard 1 Id was not found");
            Assert.AreEqual(1, standardsReturned.Count(x => x.StandardId == _standardCollation2.StandardId), "Standard 2 Id was not found");
        }

        [Test]
        public void CheckFullStandardDetailsAreReturned()
        {
           var standard = _repository.GetStandardCollationByStandardId(_standardId1).Result;

           Assert.AreEqual(_standardId1,standard.StandardId);
           Assert.AreEqual(_title1, standard.Title);
           Assert.AreEqual(_referenceNumber1, standard.ReferenceNumber);
           Assert.AreEqual(_standardDataLevel1, standard.StandardData.Level);
        }

        [TestCase(1,true)]
        [TestCase(10, true)]
        [TestCase(11, false)]
        [TestCase(null, false)]
        public void GetStandardByIdAndCheckTheOrganisationIsReturnedIfExpected(int standardId, bool expectedReturned)
        {
            var isReturned = _repository.GetStandardCollationByStandardId(standardId).Result !=null;
            Assert.AreEqual(expectedReturned, isReturned, $"The result for StandardId: [{standardId}] is not as expected");          
        }


        [TestCase("ST0001", true)]
        [TestCase("ST0010", true)]
        [TestCase("xyz", false)]
        [TestCase(null, false)]
        public void GetStandardByReferenceNameAndCheckTheOrganisationIsReturnedIfExpected(string referenceNumber, bool expectedReturned)
        {
            var isReturned = _repository.GetStandardCollationByReferenceNumber(referenceNumber).Result != null;
            Assert.AreEqual(expectedReturned, isReturned, $"The result for Reference Number: [{referenceNumber}] is not as expected");
        }

        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            StandardCollationHandler.DeleteAllRecords();
        }
    }
}
    

