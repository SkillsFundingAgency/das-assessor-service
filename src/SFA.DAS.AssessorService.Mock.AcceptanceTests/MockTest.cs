using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SFA.DAS.AssessorService.Mock.AcceptanceTests
{
    [TestClass]
    public class MockTest
    {
        [TestMethod]
        public void MaxNumberTest()
        {
            var num1 = 3;
            var num2 = 2;

            var result = Math.Max(num1, num2);

            Assert.AreEqual(num1, result);
        }
    }
}