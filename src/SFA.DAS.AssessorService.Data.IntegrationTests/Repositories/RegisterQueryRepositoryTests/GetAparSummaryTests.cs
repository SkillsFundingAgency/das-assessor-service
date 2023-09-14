using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.RegisterQueryRepositoryTests
{
    public class GetAparSummaryTests : TestBase
    {
        private static DateTime DefaultEffectiveFrom = DateTime.Today.AddDays(-50);
        private static DateTime DefaultDateStandardApprovedOnRegister = DateTime.Today.AddDays(-45);

        [Test]
        public async Task GetAparSummary_IncludesOrganisations_WhenOrganisationHasAtLeastOneStandardVersion()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_ExcludesOrganisations_WhenOrganisationDoesNotHaveAtLeastOneStandardVersion()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)) // no standard version
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsNotContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesOrganisations_WhenOrganisationStandardEffectiveToDateNotSet()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesOrganisations_WhenOrganisationStandardEffectiveToDateInFuture()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, DateTime.Now.AddDays(1), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, DateTime.Now.AddDays(1), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_ExcludesOrganisations_WhenOrganisationStandardEffectiveToInPast()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, DateTime.Now.AddDays(1), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, DateTime.Now.AddDays(-1), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsNotContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesOrganisations_WhenOrganisationStandardVersionEffectiveToDateNotSet()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();
                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesOrganisations_WhenOrganisationStandardVersionEffectiveToDateInFuture()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, DateTime.Now.AddDays(1))
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, DateTime.Now.AddDays(1)))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_ExcludesOrganisations_WhenOrganisationStandardVersionEffectiveToInPast()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, DateTime.Now.AddDays(-1)))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsNotContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesOrganisations_WhenStandardEffectiveToDateNotSet()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard1", "ST0001", 101, "1.1", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.1", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesOrganisations_WhenStandardEffectiveToDateInFuture()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard1", "ST0001", 101, "1.1", DateTime.Today.AddDays(1))
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.1", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_ExcludesOrganisations_WhenStandardEffectiveToDateInPast()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard1", "ST0001", 101, "1.1", DateTime.Today.AddDays(-1))
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.1", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsNotContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesStandards_WhenOrganisationStandardHasAtLeastOneStandardVersion()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAparSummary_ExcludesStandards_WhenOrganisationDoesNotHaveAtLeastOneStandardVersion()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister) // no standard version
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom.AddDays(5), DefaultDateStandardApprovedOnRegister.AddDays(5)); // the later standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesStandards_WhenOrganisationStandardVersionHasEffectiveToDateNotSet()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesStandards_WhenOrganisationStandardVersionHasEffectiveToDateInFuture()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, DateTime.Today.AddDays(10))
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAparSummary_ExcludesStandards_WhenOrganisationStandardVersionHasEffectiveToDateInPast()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, DateTime.Today.AddDays(-10))
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom.AddDays(5), DefaultDateStandardApprovedOnRegister.AddDays(5)); // the later standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesStandards_WhenOrganisationStandardHasEffectiveToDateNotSet()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesStandards_WhenOrganisationStandardHasEffectiveToDateInFuture()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, DateTime.Today.AddDays(10), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAparSummary_ExcludesStandards_WhenOrganisationStandardHasEffectiveToDateInPast()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, DateTime.Today.AddDays(-10), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom.AddDays(5), DefaultDateStandardApprovedOnRegister.AddDays(5)); // the later standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesStandards_WhenStandardHasEffectiveToDateNotSet()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAparSummary_IncludesStandards_WhenStandardHasEffectiveToDateInFuture()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", DateTime.Today.AddDays(10))
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAparSummary_ExcludesStandards_WhenStandardHasEffectiveToDateInPast()
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", DateTime.Today.AddDays(-10))
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom.AddDays(5), DefaultDateStandardApprovedOnRegister.AddDays(5)); // the later standard is chosen
                }
            }
        }

        [TestCase(123456, "EPA0001")]
        [TestCase(123456, "EPA0001")]
        [TestCase(123456, "EPA0001")]
        public async Task GetAparSummary_IncludesOrganisationForSpecificUkprn_WhenOrganisationHasAtLeastOneStandardVersion(int ukprn, string expectedEndPointAssessorOrganisationId)
        {
            using (var fixture = new GetAparSummaryTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345, null)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation3", "EPA0003", 561234, null)
                .WithOrganisationStandard(3, "EPA0003", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0003", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAparSummary(ukprn);

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain(expectedEndPointAssessorOrganisationId);
                }
            }
        }

        private class GetAparSummaryTestsFixture : FixtureBase<GetAparSummaryTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private RegisterQueryRepository _repository;
            public IEnumerable<AparSummary> _results;

            public GetAparSummaryTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
                _repository = new RegisterQueryRepository(new UnitOfWork(_sqlConnection));
            }

            public async Task<GetAparSummaryTestsFixture> GetAparSummary()
            {
                await _repository.AparSummaryUpdate();
                _results = await _repository.GetAparSummary();
                return this;
            }

            public async Task<GetAparSummaryTestsFixture> GetAparSummary(int ukprn)
            {
                await _repository.AparSummaryUpdate();
                _results = await _repository.GetAparSummary(ukprn);
                return this;
            }

            public GetAparSummaryTestsFixture VerifyNumberOfResults(int numberOfResults)
            {
                _results.Count().Should().Be(numberOfResults);
                return this;
            }

            public GetAparSummaryTestsFixture VerifyResultsNotContain(string endPointAssessorOrganisationId)
            {
                _results.Should().NotContain(o => o.Id == endPointAssessorOrganisationId);
                return this;
            }

            public GetAparSummaryTestsFixture VerifyResultsContain(string endPointAssessorOrganisationId)
            {
                _results.Should().Contain(o => o.Id == endPointAssessorOrganisationId);
                return this;
            }

            public GetAparSummaryTestsFixture VerifyResultsContain(string endPointAssessorOrganisationId, DateTime? earlistStandardEffectiveFromDate, DateTime? earliestDateStandardApprovedOnRegister)
            {
                _results.Should().Contain(o => o.Id == endPointAssessorOrganisationId
                    && o.EarliestEffectiveFromDate == earlistStandardEffectiveFromDate
                    && o.EarliestDateStandardApprovedOnRegister == earliestDateStandardApprovedOnRegister);
                return this;
            }
        }
    }
}