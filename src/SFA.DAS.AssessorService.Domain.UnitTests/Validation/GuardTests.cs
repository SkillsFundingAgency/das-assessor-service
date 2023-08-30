using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.AssessorService.Domain.Validation;

namespace SFA.DAS.AssessorService.Domain.UnitTests.Validation
{
    public class GuardTests
    {
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void ThrowsIfNullOrWhiteSpace(string str)
        {
            Assert.That(() => Guard.NotNullOrWhiteSpace(str), Throws.ArgumentException);
        }

        [TestCase("blah")]
        [TestCase("12345")]
        public void DoesNotThrowForNonEmptyStrings(string str)
        {
            Assert.That(() => Guard.NotNullOrWhiteSpace(str), Throws.Nothing);
        }
    }
}
