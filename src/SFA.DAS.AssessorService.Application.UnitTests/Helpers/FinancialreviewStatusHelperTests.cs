using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Helpers;
using System;
using System.Collections;

namespace SFA.DAS.AssessorService.Application.UnitTests.Helpers
{
    public class FinancialreviewStatusHelperTests
    {
        // Hard coded dates instead of relative to Now() because tests need to be discoverable by TestRunner at build time.
        // See: https://github.com/nunit/nunit3-vs-adapter/issues/734
        private static DateTime? pastDate = new DateTime(2020, 01, 01);
        private static DateTime? futureDate = new DateTime(2999, 01, 01);

        public class FinancialExemptionCases
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return new TestCaseData(null, null, new OrganisationType() { FinancialExempt = false }).Returns(false);
                    yield return new TestCaseData(null, null, new OrganisationType() { FinancialExempt = true }).Returns(true);

                    yield return new TestCaseData(null, futureDate, new OrganisationType() { FinancialExempt = false }).Returns(true);
                    yield return new TestCaseData(null, futureDate, new OrganisationType() { FinancialExempt = true }).Returns(true);

                    yield return new TestCaseData(null, pastDate, new OrganisationType() { FinancialExempt = false }).Returns(false);
                    yield return new TestCaseData(null, pastDate, new OrganisationType() { FinancialExempt = true }).Returns(true);



                    yield return new TestCaseData(false, null, new OrganisationType() { FinancialExempt = false }).Returns(false);
                    yield return new TestCaseData(false, null, new OrganisationType() { FinancialExempt = true }).Returns(true);

                    yield return new TestCaseData(false, futureDate, new OrganisationType() { FinancialExempt = false }).Returns(true);
                    yield return new TestCaseData(false, futureDate, new OrganisationType() { FinancialExempt = true }).Returns(true);

                    yield return new TestCaseData(false, pastDate, new OrganisationType() { FinancialExempt = false }).Returns(false);
                    yield return new TestCaseData(false, pastDate, new OrganisationType() { FinancialExempt = true }).Returns(true);



                    yield return new TestCaseData(true, null, new OrganisationType() { FinancialExempt = false }).Returns(true);
                    yield return new TestCaseData(true, null, new OrganisationType() { FinancialExempt = true }).Returns(true);

                    yield return new TestCaseData(true, futureDate, new OrganisationType() { FinancialExempt = false }).Returns(true);
                    yield return new TestCaseData(true, futureDate, new OrganisationType() { FinancialExempt = true }).Returns(true);

                    yield return new TestCaseData(true, pastDate, new OrganisationType() { FinancialExempt = false }).Returns(true);
                    yield return new TestCaseData(true, pastDate, new OrganisationType() { FinancialExempt = true }).Returns(true);
                }
            }
        }

        [Test]
        [TestCaseSource(typeof(FinancialExemptionCases), nameof(FinancialExemptionCases.TestCases))]
        public bool IsFinancialExempt_ReturnsBool(bool? financialExempt, DateTime? financialDueDate, OrganisationType orgType)
        {
            return FinancialReviewStatusHelper.IsFinancialExempt(financialExempt, financialDueDate, orgType);
        }
    }
}
