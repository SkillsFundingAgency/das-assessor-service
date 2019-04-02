using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services
{
    [TestFixture]
    public class AssessorValidationServiceTests
    {
        private AssessorValidationService _assessorValidationService;
        private Mock<IRegisterValidationRepository> _registerValidationRepository;
        [SetUp]
        public void Setup()
        {
            _registerValidationRepository = new Mock<IRegisterValidationRepository>();

            _assessorValidationService = new AssessorValidationService(_registerValidationRepository.Object);
        }

        [TestCase(true)]
        [TestCase(false)]
        public  void CheckContactDetailsAlreadyPresent(bool result)
        {
            _registerValidationRepository.Setup(r => r.ContactDetailsAlreadyExist(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),It.IsAny<Guid?>()))
                .Returns(Task.FromResult(result));
            var checkResult = _assessorValidationService.CheckIfContactDetailsAlreadyPresentInSystem("a","b","c",null ).Result;
            Assert.AreEqual(checkResult,result);
        }

        [TestCase("",false,false, false)]
        [TestCase("", true, false, false)]
        [TestCase(" ", false, false, false)]
        [TestCase(" ", true, false, false)]
        [TestCase(null, false, false, false)]
        [TestCase(null, true, false, false)]
        [TestCase("co", false, true, false)]
        [TestCase("co", true, true, true)]
        public void CheckIfCompanyNumberAlreadyTaken(string companyNumber, bool repositoryResponse, bool repositoryCalled, bool expectedResult)
        {
            _registerValidationRepository.Setup(r => r.EpaOrganisationExistsWithCompanyNumber(It.IsAny<string>()))
                .Returns(Task.FromResult(repositoryResponse));
            var checkResult = _assessorValidationService.IsCompanyNumberTaken(companyNumber).Result;
            Assert.AreEqual(expectedResult,checkResult);
            if (repositoryCalled)
                _registerValidationRepository.Verify(r => r.EpaOrganisationExistsWithCompanyNumber(It.IsAny<string>()));
            else
            _registerValidationRepository.Verify(r => r.EpaOrganisationExistsWithCompanyNumber(It.IsAny<string>()),Times.Never);
        }

        [TestCase("", false, false, false)]
        [TestCase("", true, false, false)]
        [TestCase(" ", false, false, false)]
        [TestCase(" ", true, false, false)]
        [TestCase(null, false, false, false)]
        [TestCase(null, true, false, false)]
        [TestCase("a", false, true, false)]
        [TestCase("a", true, true, true)]
        public void CheckIfCharityNumberAlreadyTaken(string charityNumber, bool repositoryResponse, bool repositoryCalled, bool expectedResult)
        {
            _registerValidationRepository.Setup(r => r.EpaOrganisationExistsWithCharityNumber(It.IsAny<string>()))
                .Returns(Task.FromResult(repositoryResponse));
            var checkResult = _assessorValidationService.IsCharityNumberTaken(charityNumber).Result;
            Assert.AreEqual(expectedResult, checkResult);
            if (repositoryCalled)
                _registerValidationRepository.Verify(r => r.EpaOrganisationExistsWithCharityNumber(It.IsAny<string>()));
            else
                _registerValidationRepository.Verify(r => r.EpaOrganisationExistsWithCharityNumber(It.IsAny<string>()), Times.Never);
        }


        [TestCase("", false, false, false)]
        [TestCase("", true, false, false)]
        [TestCase(" ", false, false, false)]
        [TestCase(" ", true, false, false)]
        [TestCase(null, false, false, false)]
        [TestCase(null, true, false, false)]
        [TestCase("email", false, true, false)]
        [TestCase("email", true, true, true)]
        public void CheckIfEmailAlreadyTaken(string email, bool repositoryResponse, bool repositoryCalled, bool expectedResult)
        {
            _registerValidationRepository.Setup(r => r.EmailAlreadyPresent(It.IsAny<string>()))
                .Returns(Task.FromResult(repositoryResponse));
            var checkResult = _assessorValidationService.IsEmailTaken(email).Result;
            Assert.AreEqual(expectedResult, checkResult);
            if (repositoryCalled)
                _registerValidationRepository.Verify(r => r.EmailAlreadyPresent(It.IsAny<string>()));
            else
                _registerValidationRepository.Verify(r => r.EmailAlreadyPresent(It.IsAny<string>()), Times.Never);
        }


        [TestCase("", false, false, false)]
        [TestCase("", true, false, false)]
        [TestCase(" ", false, false, false)]
        [TestCase(" ", true, false, false)]
        [TestCase(null, false, false, false)]
        [TestCase(null, true, false, false)]
        [TestCase("orgName", false, true, false)]
        [TestCase("organisation Name", true, true, true)]
        public void CheckIOrganisationNameAlreadyTaken(string email, bool repositoryResponse, bool repositoryCalled, bool expectedResult)
        {
            _registerValidationRepository.Setup(r => r.EpaOrganisationAlreadyUsingName(It.IsAny<string>(),string.Empty))
                .Returns(Task.FromResult(repositoryResponse));
            var checkResult = _assessorValidationService.IsOrganisationNameTaken(email).Result;
            Assert.AreEqual(expectedResult, checkResult);
            if (repositoryCalled)
                _registerValidationRepository.Verify(r => r.EpaOrganisationAlreadyUsingName(It.IsAny<string>(),string.Empty));
            else
                _registerValidationRepository.Verify(r => r.EpaOrganisationAlreadyUsingName(It.IsAny<string>(),string.Empty), Times.Never);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CheckIfUkprnAlreadyTaken(bool result)
        {
            _registerValidationRepository.Setup(r => r.EpaOrganisationExistsWithUkprn(It.IsAny<long>()))
                .Returns(Task.FromResult(result));
            var checkResult = _assessorValidationService.IsOrganisationUkprnTaken(1234).Result;
            Assert.AreEqual(checkResult, result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsOrganisationStandardTaken(bool result)
        {
            _registerValidationRepository.Setup(r => r.EpaOrganisationStandardExists(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Task.FromResult(result));
            var checkResult = _assessorValidationService.IsOrganisationStandardTaken("EPA0001", 1).Result;
            Assert.AreEqual(checkResult, result);
        }
    }
}
