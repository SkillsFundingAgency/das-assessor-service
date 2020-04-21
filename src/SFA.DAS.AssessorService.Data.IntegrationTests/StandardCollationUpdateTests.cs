using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    [TestFixture]
    public class StandardCollationUpdateTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        
        private SqlConnection _databaseConnection;
        private UnitOfWork _unitOfWork;
        private StandardRepository _repository;

        private static List<StandardCollationModel> _existingStandardCollations = new List<StandardCollationModel>();
        private static List<StandardCollationModel> _latestStandardCollationsWithSecondStandardRemoved = new List<StandardCollationModel>();

        private const int _firstStandardId = 1;
        private const int _secondStandardId = 10;
        private const int _thirdStandardId = 100;
        private const int _fourthStandardId = 200;

        private const string _firstStandardReference = "ST0001";
        private const string _secondStandardReference = "ST0010";
        private const string _thirdStandardReference = "ST0100";
        private const string _fourthStandardReference = "ST0200";

        private const int _firstNewStandardId = 400;
        private const string _firstNewStandardReference = "ST0400";
        private const string _firstNewStandardTitle = "Standard title 400";
        private const string _firstNewStandardData = "{\"Level\":1}";
        private const string _firstNewStandardOptionName = "Option 400.1";
        private const int _firstNewStandardIsLive = 1;

        [SetUp]
        public void SetupStandardCollationTests()
        {
            _databaseConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
            _unitOfWork = new UnitOfWork(_databaseConnection);            
            _repository = new StandardRepository(_unitOfWork);

            _existingStandardCollations = new List<StandardCollationModel> {
                new StandardCollationModel
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
                },
                new StandardCollationModel
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
                },
                new StandardCollationModel
                {
                    StandardId = _thirdStandardId,
                    ReferenceNumber = _thirdStandardReference,
                    Title = $"Standard title {_thirdStandardId}",
                    StandardData = null,
                    Options = new List<OptionDataModel>(),
                    IsLive = 1
                },
                new StandardCollationModel
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
                }
            };

            StandardCollationHandler.InsertRecords(_existingStandardCollations);
        }

        [TestCase(_firstStandardId, true, false)]
        [TestCase(_secondStandardId, true, false)]
        [TestCase(_thirdStandardId, true, false)]
        [TestCase(_fourthStandardId, false, false)]
        public async Task UpdateStandardCollationToRemoveStandard(int standardId, bool isLiveBefore, bool isLiveAfter)
        {
            var _lastestStandardCollations = Clone(_existingStandardCollations);
            _lastestStandardCollations.RemoveAll(p => p.StandardId == standardId);

            var beforeStandard = await _repository.GetStandardCollationByStandardId(standardId);
            var result = await _repository.UpsertApprovedStandards(_lastestStandardCollations);
            var afterStandard = await _repository.GetStandardCollationByStandardId(standardId);

            Assert.AreEqual(isLiveBefore, beforeStandard != null);
            Assert.AreEqual(isLiveAfter, afterStandard != null);
        }

        [TestCase(_firstStandardId, true, true, "Updated Title 1")]
        [TestCase(_secondStandardId, true, true, "Updated Title 2")]
        [TestCase(_thirdStandardId, true, true, "Updated Title 3")]
        [TestCase(_fourthStandardId, false, true, "Updated Title 4")]
        public async Task UpdateStandardCollationToUpdateStandardTitle(int standardId, bool isLiveBefore, bool isLiveAfter, string updatedTitle)
        {
            var existingStandard = _existingStandardCollations.Where(p => p.StandardId == standardId).Single();
            var _lastestStandardCollations = Clone(_existingStandardCollations);
           
            _lastestStandardCollations.Where(p => p.StandardId == standardId).Single().Title = updatedTitle;            

            var beforeStandard = await _repository.GetStandardCollationByStandardId(standardId);
            var result = await _repository.UpsertApprovedStandards(_lastestStandardCollations);
            var afterStandard = await _repository.GetStandardCollationByStandardId(standardId);

            Assert.AreEqual(isLiveBefore, beforeStandard != null);
            if (isLiveBefore)
            {
                Assert.AreEqual(beforeStandard.Title, existingStandard.Title);
            }
            
            Assert.AreEqual(isLiveAfter, afterStandard != null);
            Assert.AreEqual(afterStandard.Title, updatedTitle);
        }

        [TestCase(_firstNewStandardId, _firstNewStandardReference, _firstNewStandardTitle, _firstNewStandardData, _firstNewStandardOptionName)]
        public async Task UpdateStandardCollationToInsertStandard(int standardId, string referenceNumber, string title, string standardData, string optionName)
        {
            var _lastestStandardCollations = Clone(_existingStandardCollations);
            var newStandard = new StandardCollation
            {
                StandardId = standardId,
                ReferenceNumber = referenceNumber,
                Title = title,
                StandardData = JsonConvert.DeserializeObject<StandardData>(standardData),
                Options = new List<string>
                {
                    optionName
                }
            };

            _lastestStandardCollations.Add(newStandard);

            var result = await _repository.UpsertApprovedStandards(_lastestStandardCollations);
            var insertedStandard = await _repository.GetStandardCollationByStandardId(standardId);

            Assert.AreEqual(newStandard.StandardId, insertedStandard.StandardId);
        }

        [TearDown]
        public void TearDownStandardCollationTests()
        {
            StandardCollationHandler.DeleteAllRecords();

            if(_databaseConnection != null)
            {
                _databaseConnection.Dispose();
            }
        }

        private List<StandardCollation> Clone(List<StandardCollationModel> existingStandardCollations)
        {
            var latestStandardCollection = existingStandardCollations.ConvertAll(p => new StandardCollation
            {
                Id = 0,
                StandardId = p.StandardId,
                ReferenceNumber = p.ReferenceNumber,
                StandardData = p.StandardData != null ? JsonConvert.DeserializeObject<StandardData>(p.StandardData) : null,
                Title = p.Title,
                Options = p.Options.ConvertAll(o => o.OptionName)
            });

            return latestStandardCollection;
        }
    }
}
    

