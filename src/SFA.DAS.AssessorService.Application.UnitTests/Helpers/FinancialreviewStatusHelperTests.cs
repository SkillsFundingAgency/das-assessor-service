using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Helpers;
using System;
using System.Collections;

namespace SFA.DAS.AssessorService.Application.UnitTests.Helpers
{
    public class FinancialreviewStatusHelperTests
    {
        private static DateTime? pastDate = DateTime.Now.AddYears(-1);
        private static DateTime? futureDate = DateTime.Now.AddYears(1);

        public class FinancialExemptionCases
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return new TestCaseData(null, null, new OrganisationType() { FinancialExempt = false }).Returns(false).SetName("Null, Null, Not Exempt, Returns False");
                    yield return new TestCaseData(null, null, new OrganisationType() { FinancialExempt = true }).Returns(true).SetName("Null, Null, Exempt, Returns True");

                    yield return new TestCaseData(null, futureDate, new OrganisationType() { FinancialExempt = false }).Returns(true).SetName("Null, FutureDate, Not Exempt, Returns True");
                    yield return new TestCaseData(null, futureDate, new OrganisationType() { FinancialExempt = true }).Returns(true).SetName("Null, Future Date, Exempt, Returns True");

                    yield return new TestCaseData(null, pastDate, new OrganisationType() { FinancialExempt = false }).Returns(false).SetName("Null, Past Date, Not Exempt, Returns False");
                    yield return new TestCaseData(null, pastDate, new OrganisationType() { FinancialExempt = true }).Returns(true).SetName("Null, Past Date, Exempt, Returns True");



                    yield return new TestCaseData(false, null, new OrganisationType() { FinancialExempt = false }).Returns(false).SetName("False, Null, Not Exempt, Returns False");
                    yield return new TestCaseData(false, null, new OrganisationType() { FinancialExempt = true }).Returns(true).SetName("False, Null, Exempt, Returns True");

                    yield return new TestCaseData(false, futureDate, new OrganisationType() { FinancialExempt = false }).Returns(true).SetName("False, Future Date, Not Exempt, Returns True");
                    yield return new TestCaseData(false, futureDate, new OrganisationType() { FinancialExempt = true }).Returns(true).SetName("False, Future Date, Exempt, Returns True");

                    yield return new TestCaseData(false, pastDate, new OrganisationType() { FinancialExempt = false }).Returns(false).SetName("False, Past Date, Not Exempt, Returns False");
                    yield return new TestCaseData(false, pastDate, new OrganisationType() { FinancialExempt = true }).Returns(true).SetName("False, Past Date, Exempt, Returns True");



                    yield return new TestCaseData(true, null, new OrganisationType() { FinancialExempt = false }).Returns(true).SetName("True, Null, Not Exempt, Returns True");
                    yield return new TestCaseData(true, null, new OrganisationType() { FinancialExempt = true }).Returns(true).SetName("True, Null, Exempt, Returns True");

                    yield return new TestCaseData(true, futureDate, new OrganisationType() { FinancialExempt = false }).Returns(true).SetName("True, Future Date, Not Exempt, Returns True");
                    yield return new TestCaseData(true, futureDate, new OrganisationType() { FinancialExempt = true }).Returns(true).SetName("True, Future Date, Exempt, Returns True");

                    yield return new TestCaseData(true, pastDate, new OrganisationType() { FinancialExempt = false }).Returns(true).SetName("True, Past Date, Not Exempt, Returns True");
                    yield return new TestCaseData(true, pastDate, new OrganisationType() { FinancialExempt = true }).Returns(true).SetName("True, Past Date, Exempt, Returns True");
                }
            }
        }

        [TestCaseSource(typeof(FinancialExemptionCases), nameof(FinancialExemptionCases.TestCases))]
        public bool IsFinancialExempt_ReturnsBool(bool? financialExempt, DateTime? financialDueDate, OrganisationType orgType)
        {
            return FinancialReviewStatusHelper.IsFinancialExempt(financialExempt, financialDueDate, orgType);
        }
    }
}
