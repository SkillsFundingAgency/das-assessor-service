using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;

namespace SFA.DAS.AssessorService.Web.UnitTests.ViewModelTests.ApplicationResponseViewModelTests
{
    public class WhenCallingApplicationPostAction
    {
        [TestCase(true, nameof(ApplicationController.FinancialExpiredRedir))]
        [TestCase(false, nameof(ApplicationController.StartApplication))]
        public void ActionNameIsReturned_BasedOn_FinancialInfoStage1Expired(bool isExpired, string actionMethodName)
        {
            var sut = new ApplicationResponseViewModel() { FinancialInfoStage1Expired = isExpired };
            Assert.That(sut.ApplicationPostAction(), Is.EqualTo(actionMethodName));
        }
    }
}
