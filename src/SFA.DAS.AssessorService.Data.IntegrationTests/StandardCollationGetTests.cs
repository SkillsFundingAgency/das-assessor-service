using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    [TestFixture]
    public class StandardCollationGetTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        
        private SqlConnection _databaseConnection;
        private UnitOfWork _unitOfWork;
        private StandardRepository _repository;

        private static List<StandardCollationModel> _standardCollations = new List<StandardCollationModel>();

        private const int _firstStandardId = 1;
        private const int _secondStandardId = 10;
        private const int _thirdStandardId = 100;
        private const int _fourthStandardId = 200;

        private const string _firstStandardReference = "ST0001";
        private const string _secondStandardReference = "ST0010";
        private const string _thirdStandardReference = "ST0100";
        private const string _fourthStandardReference = "ST0200";

        [OneTimeSetUp]
        public void SetupStandardCollationTests()
        {
            _databaseConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
            _unitOfWork = new UnitOfWork(_databaseConnection);            
            _repository = new StandardRepository(_unitOfWork);
           
            _standardCollations.Add(new StandardCollationModel
            {
                StandardId = _firstStandardId,
                ReferenceNumber = _firstStandardReference,
                Title = $"Standard title {_firstStandardId}",
                StandardData = JsonConvert.SerializeObject(new StandardDataModel
                {
                    Level = 1
                }),
                Options = new List<OptionDataModel>
                {
                    new OptionDataModel
                    {
                        StdCode = _firstStandardId,
                        OptionName = $"Option {_firstStandardId}.1",
                        IsLive = 1
                    },
                    new OptionDataModel
                    {
                        StdCode = 1,
                        OptionName = $"Option {_firstStandardId}.2",
                        IsLive = 1
                    },
                    new OptionDataModel
                    {
                        StdCode = 1,
                        OptionName = $"Option {_firstStandardId}.3",
                        IsLive = 0
                    }
                },
                IsLive = 1
            });

            _standardCollations.Add(new StandardCollationModel
            {
                StandardId = _secondStandardId,
                ReferenceNumber = _secondStandardReference,
                Title = $"Standard title {_secondStandardId}",
                StandardData = JsonConvert.SerializeObject(new StandardDataModel
                {
                    Level = 2
                }),
                Options = new List<OptionDataModel>
                {
                    new OptionDataModel
                    {
                        StdCode = _secondStandardId,
                        OptionName = $"Option {_secondStandardId}.1",
                        IsLive = 1
                    },
                    new OptionDataModel
                    {
                        StdCode = _secondStandardId,
                        OptionName = $"Option {_secondStandardId}.2",
                        IsLive = 1
                    }
                },
                IsLive = 1
            });

            _standardCollations.Add(new StandardCollationModel
            {
                StandardId = _thirdStandardId,
                ReferenceNumber = _thirdStandardReference,
                Title = $"Standard title {_thirdStandardId}",
                StandardData = null,
                Options = new List<OptionDataModel>(),
                IsLive = 1
            });

            _standardCollations.Add(new StandardCollationModel
            {
                StandardId = _fourthStandardId,
                ReferenceNumber = _fourthStandardReference,
                Title = $"Standard title {_fourthStandardId}",
                StandardData = JsonConvert.SerializeObject(new StandardDataModel
                {
                    Level = 4
                }),
                Options = new List<OptionDataModel>
                {
                    new OptionDataModel
                    {
                        StdCode = _fourthStandardId,
                        OptionName = $"Option {_fourthStandardId}.1",
                        IsLive = 0
                    },
                    new OptionDataModel
                    {
                        StdCode = _fourthStandardId,
                        OptionName = $"Option {_fourthStandardId}.2",
                        IsLive = 1
                    }
                },
                IsLive = 0
            });

            StandardCollationHandler.InsertRecords(_standardCollations);
        }

        //[Test, Ignore("Temporary ignore during 3.1 upgrade due to Invalid object name 'dbo.StandardCollation'")]
        public async Task RunGetAllStandardsAndCheckAllStandardsExpectedAreReturned()
        {
            var standardsReturned = (await _repository.GetStandardCollations()).ToList();

            var liveStandardCollationsCount = _standardCollations.Where(p => p.IsLive == 1).Count();
            Assert.AreEqual(liveStandardCollationsCount, standardsReturned.Count, $"Expected {liveStandardCollationsCount} standards back but got {standardsReturned.Count}");
            
            foreach(var standardModel in _standardCollations.Where(p => p.IsLive == 1))
            {
                Assert.AreEqual(1, standardsReturned.Count(x => x.StandardId == standardModel.StandardId), $"Standard Id: {standardModel.StandardId} - was not found");
            }
        }

        //[TestCase(_firstStandardId), Ignore("Temporary ignore during 3.1 upgrade due to Invalid object name 'dbo.StandardCollation'")]
        //[TestCase(_secondStandardId)]
        //[TestCase(_thirdStandardId)]
        public async Task CheckFullStandardDetailsAreReturned(int standardId)
        {
            var standardExpected = _standardCollations.Single(p => p.StandardId == standardId);
            var standardReturned = (await _repository.GetStandardCollationByStandardId(standardId));
            
            Assert.AreEqual(standardExpected.StandardId, standardReturned.StandardId);
            Assert.AreEqual(standardExpected.Title, standardReturned.Title);
            Assert.AreEqual(standardExpected.ReferenceNumber, standardReturned.ReferenceNumber);
            
            if (standardExpected.StandardData != null)
            {
                Assert.AreEqual(JsonConvert.DeserializeObject<StandardDataModel>(standardExpected.StandardData).Level, standardReturned.StandardData.Level);
            }
            else
            {
                Assert.IsNull(standardReturned.StandardData);
            }
        }

        //[TestCase(_firstStandardId,true), Ignore("Temporary ignore during 3.1 upgrade due to Invalid object name 'dbo.StandardCollation'")]
        //[TestCase(_secondStandardId, true)]
        //[TestCase(_thirdStandardId, true)]
        //[TestCase(_fourthStandardId, false)] // the standard exists but is not live
        //[TestCase(null, false)]
        public async Task GetStandardByIdAndCheckTheStandardCollationIsFoundIfExpected(int standardId, bool found)
        {
            var standardReturned = await _repository.GetStandardCollationByStandardId(standardId);
            Assert.AreEqual(found, standardReturned != null, $"The result for StandardId: [{standardId}] is not as expected");          
        }


        //[TestCase(_firstStandardReference, true), Ignore("Temporary ignore during 3.1 upgrade due to Invalid object name 'dbo.StandardCollation'")]        
        //[TestCase(_secondStandardReference, true)]
        //[TestCase(_thirdStandardReference, true)]
        //[TestCase(_fourthStandardReference, false)] // the standard exists but is not live
        //[TestCase("xyz", false)]
        public async Task GetStandardByReferenceNumberAndCheckTheStandardCollationIsFoundIfExpected(string referenceNumber, bool found)
        {
            var standardReturned = await _repository.GetStandardCollationByReferenceNumber(referenceNumber);
            Assert.AreEqual(found, standardReturned != null, $"The result for Reference Number: [{referenceNumber}] is not as expected");
        }

        //[TestCase(_firstStandardId, 2), Ignore("Temporary ignore during 3.1 upgrade due to Invalid object name 'dbo.StandardCollation'")]
        //[TestCase(_secondStandardId, 2)]
        //[TestCase(_thirdStandardId, 0)]
        public async Task GetStandardByIdAndCheckThatLiveOptionsOnlyAreReturned(int standardId, int liveOptionsCount)
        {
            var standardReturned = await _repository.GetStandardCollationByStandardId(standardId);
            Assert.AreEqual(liveOptionsCount, standardReturned.Options.Count, $"The result for StandardId: [{standardId}] has incorrect number of options {liveOptionsCount}");
        }

        //[TestCase(_firstStandardReference, 2), Ignore("Temporary ignore during 3.1 upgrade due to Invalid object name 'dbo.StandardCollation'")]
        //[TestCase(_secondStandardReference, 2)]
        //[TestCase(_thirdStandardReference, 0)]
        public async Task GetStandardByReferenceNumberAndCheckThatLiveOptionsOnlyAreReturned(string referenceNumber, int liveOptionsCount)
        {
            var standardReturned = await _repository.GetStandardCollationByReferenceNumber(referenceNumber);
            Assert.AreEqual(liveOptionsCount, standardReturned.Options.Count, $"The result for Reference Number: [{referenceNumber}] has incorrect number of options {liveOptionsCount}");
        }

        [OneTimeTearDown]
        public void TearDownStandardCollationTests()
        {
            StandardCollationHandler.DeleteAllRecords();

            if(_databaseConnection != null)
            {
                _databaseConnection.Dispose();
            }
        }
    }
}
    

