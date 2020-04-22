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

        
        private const int _firstExistingStandardId = 1;
        private const string _firstExistingStandardReference = "ST0001";
        private const string _firstExistingStandardFirstOptionName = "Option 1.1";
        private const string _firstExistingStandardThirdOptionName = "Option 1.3";
        private const string _firstExistingStandardFourthNewOptionName = "Option 1.4";
        
        private const int _secondExistingStandardId = 10;
        private const string _secondExistingStandardReference = "ST0010";
        private const string _secondExistingStandardFirstOptionName = "Option 2.1";
        private const string _secondExistingStandardThirdNewOptionName = "Option 2.3";

        private const int _thirdExistingStandardId = 100;
        private const string _thirdExistingStandardReference = "ST0100";
        
        private const int _fourthExistingStandardId = 200;
        private const string _fourthExistingStandardReference = "ST0200";

        private const int _fifthNewStandardId = 400;
        private const string _fifthNewStandardReference = "ST0400";
        private const string _fifthNewStandardTitle = "Standard title 400";
        private const string _fifthNewStandardData = "{\"Level\":1}";
        private const string _fifthNewStandardOptionName = "Option 400.1";

        [SetUp]
        public void SetupStandardCollationTests()
        {
            _databaseConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
            _unitOfWork = new UnitOfWork(_databaseConnection);            
            _repository = new StandardRepository(_unitOfWork);

            _existingStandardCollations = new List<StandardCollationModel> {
                new StandardCollationModel
                {
                    StandardId = _firstExistingStandardId,
                    ReferenceNumber = _firstExistingStandardReference,
                    Title = $"Standard title {_firstExistingStandardId}",
                    StandardData = JsonConvert.SerializeObject(new StandardDataModel
                    {
                        Level = 1
                    }),
                    Options = new List<OptionDataModel>
                    {
                        new OptionDataModel
                        {
                            StdCode = _firstExistingStandardId,
                            OptionName = _firstExistingStandardFirstOptionName,
                            IsLive = 1
                        },
                        new OptionDataModel
                        {
                            StdCode = 1,
                            OptionName = $"Option {_firstExistingStandardId}.2",
                            IsLive = 1
                        },
                        new OptionDataModel
                        {
                            StdCode = 1,
                            OptionName = _firstExistingStandardThirdOptionName,
                            IsLive = 0
                        }
                    },
                    IsLive = 1
                },
                new StandardCollationModel
                {
                    StandardId = _secondExistingStandardId,
                    ReferenceNumber = _secondExistingStandardReference,
                    Title = $"Standard title {_secondExistingStandardId}",
                    StandardData = JsonConvert.SerializeObject(new StandardDataModel
                    {
                        Level = 2
                    }),
                    Options = new List<OptionDataModel>
                    {
                        new OptionDataModel
                        {
                            StdCode = _secondExistingStandardId,
                            OptionName = _secondExistingStandardFirstOptionName,
                            IsLive = 1
                        },
                        new OptionDataModel
                        {
                            StdCode = _secondExistingStandardId,
                            OptionName = $"Option {_secondExistingStandardId}.2",
                            IsLive = 1
                        }
                    },
                    IsLive = 1
                },
                new StandardCollationModel
                {
                    StandardId = _thirdExistingStandardId,
                    ReferenceNumber = _thirdExistingStandardReference,
                    Title = $"Standard title {_thirdExistingStandardId}",
                    StandardData = null,
                    Options = new List<OptionDataModel>(),
                    IsLive = 1
                },
                new StandardCollationModel
                {
                    StandardId = _fourthExistingStandardId,
                    ReferenceNumber = _fourthExistingStandardReference,
                    Title = $"Standard title {_fourthExistingStandardId}",
                    StandardData = JsonConvert.SerializeObject(new StandardDataModel
                    {
                        Level = 4
                    }),
                    Options = new List<OptionDataModel>
                    {
                        new OptionDataModel
                        {
                            StdCode = _fourthExistingStandardId,
                            OptionName = $"Option {_fourthExistingStandardId}.1",
                            IsLive = 0
                        },
                        new OptionDataModel
                        {
                            StdCode = _fourthExistingStandardId,
                            OptionName = $"Option {_fourthExistingStandardId}.2",
                            IsLive = 1
                        }
                    },
                    IsLive = 0
                }
            };

            StandardCollationHandler.InsertRecords(_existingStandardCollations);
        }

        [TestCase(_firstExistingStandardId, true, false)]
        [TestCase(_secondExistingStandardId, true, false)]
        [TestCase(_thirdExistingStandardId, true, false)]
        [TestCase(_fourthExistingStandardId, false, false)]
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

        [TestCase(_firstExistingStandardId, true, true, "Updated Title 1")]
        [TestCase(_secondExistingStandardId, true, true, "Updated Title 2")]
        [TestCase(_thirdExistingStandardId, true, true, "Updated Title 3")]
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

        [TestCase(_fifthNewStandardId, _fifthNewStandardReference, _fifthNewStandardTitle, _fifthNewStandardData, _fifthNewStandardOptionName)]
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
        
        [TestCase(_firstExistingStandardId, _firstExistingStandardFirstOptionName, 2, 1)]
        [TestCase(_secondExistingStandardId, _secondExistingStandardFirstOptionName, 2, 1)]
        public async Task UpdateStandardCollationToRemoveOption(int standardId, string optionNameToRemove, int optionsBeforeUpdate, int optionsAfterUpdate)
        {
            var _lastestStandardCollations = Clone(_existingStandardCollations);
            var updatedStandard = _lastestStandardCollations.Where(p => p.StandardId == standardId).Single();

            updatedStandard.Options.Remove(optionNameToRemove);

            var beforeStandard = await _repository.GetStandardCollationByStandardId(standardId);
            var result = await _repository.UpsertApprovedStandards(_lastestStandardCollations);
            var afterStandard = await _repository.GetStandardCollationByStandardId(standardId);

            Assert.IsTrue(beforeStandard.Options.Contains(optionNameToRemove));
            Assert.AreEqual(optionsBeforeUpdate, beforeStandard.Options.Count);
            Assert.IsFalse(afterStandard.Options.Contains(optionNameToRemove));
            Assert.AreEqual(optionsAfterUpdate, afterStandard.Options.Count);
        }

        [TestCase(_firstExistingStandardId, _firstExistingStandardThirdOptionName, 2, 3)]
        public async Task UpdateStandardCollationToUpdateOption(int standardId, string optionNameToUpdate, int optionsBeforeUpdate, int optionsAfterUpdate)
        {
            var _lastestStandardCollations = Clone(_existingStandardCollations);
            var updatedStandard = _lastestStandardCollations.Where(p => p.StandardId == standardId).Single();

            // updating the option is adding back a current non-live option
            updatedStandard.Options.Add(optionNameToUpdate);

            var beforeStandard = await _repository.GetStandardCollationByStandardId(standardId);
            var result = await _repository.UpsertApprovedStandards(_lastestStandardCollations);
            var afterStandard = await _repository.GetStandardCollationByStandardId(standardId);

            Assert.IsFalse(beforeStandard.Options.Contains(optionNameToUpdate));
            Assert.AreEqual(optionsBeforeUpdate, beforeStandard.Options.Count);
            Assert.IsTrue(afterStandard.Options.Contains(optionNameToUpdate));
            Assert.AreEqual(optionsAfterUpdate, afterStandard.Options.Count);
        }

        [TestCase(_firstExistingStandardId, _firstExistingStandardFourthNewOptionName, 2, 3)]
        [TestCase(_secondExistingStandardId, _secondExistingStandardThirdNewOptionName, 2, 3)]
        public async Task UpdateStandardCollationToInsertOption(int standardId, string optionNameToInsert, int optionsBeforeUpdate, int optionsAfterUpdate)
        {
            var _lastestStandardCollations = Clone(_existingStandardCollations);
            var updatedStandard = _lastestStandardCollations.Where(p => p.StandardId == standardId).Single();

            updatedStandard.Options.Add(optionNameToInsert);

            var beforeStandard = await _repository.GetStandardCollationByStandardId(standardId);
            var result = await _repository.UpsertApprovedStandards(_lastestStandardCollations);
            var afterStandard = await _repository.GetStandardCollationByStandardId(standardId);

            Assert.IsFalse(beforeStandard.Options.Contains(optionNameToInsert));
            Assert.AreEqual(optionsBeforeUpdate, beforeStandard.Options.Count);
            Assert.IsTrue(afterStandard.Options.Contains(optionNameToInsert));
            Assert.AreEqual(optionsAfterUpdate, afterStandard.Options.Count);
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
            var latestStandardCollection = existingStandardCollations
                .Where(existingStandard => existingStandard.IsLive == 1).ToList()
                .ConvertAll(p => new StandardCollation
                {
                    Id = 0,
                    StandardId = p.StandardId,
                    ReferenceNumber = p.ReferenceNumber,
                    StandardData = p.StandardData != null ? JsonConvert.DeserializeObject<StandardData>(p.StandardData) : null,
                    Title = p.Title,
                    Options = p.Options
                        .Where(o => o.IsLive == 1).ToList()
                        .ConvertAll(o => o.OptionName)
                });

            return latestStandardCollection;
        }
    }
}
    

