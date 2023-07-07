using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using FluentAssertions.Numeric;
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
    public class GetAllOrganisationsWithActiveStandardsTests : TestBase
    {
        private static DateTime DefaultEffectiveFrom = DateTime.Today.AddDays(-50);
        private static DateTime DefaultDateStandardApprovedOnRegister = DateTime.Today.AddDays(-45);

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesOrganisations_WhenOrganisationHasAtLeastOneStandardVersion()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_ExcludesOrganisations_WhenOrganisationDoesNotHaveAtLeastOneStandardVersion()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)) // no standard version
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsNotContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesOrganisations_WhenOrganisationStandardEffectiveToDateNotSet()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesOrganisations_WhenOrganisationStandardEffectiveToDateInFuture()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, DateTime.Now.AddDays(1), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, DateTime.Now.AddDays(1), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_ExcludesOrganisations_WhenOrganisationStandardEffectiveToInPast()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, DateTime.Now.AddDays(1), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, DateTime.Now.AddDays(-1), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsNotContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesOrganisations_WhenOrganisationStandardVersionEffectiveToDateNotSet()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();
                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesOrganisations_WhenOrganisationStandardVersionEffectiveToDateInFuture()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, DateTime.Now.AddDays(1))
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, DateTime.Now.AddDays(1)))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_ExcludesOrganisations_WhenOrganisationStandardVersionEffectiveToInPast()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, DateTime.Now.AddDays(-1)))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsNotContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesOrganisations_WhenStandardEffectiveToDateNotSet()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard1", "ST0001", 101, "1.1", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.1", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesOrganisations_WhenStandardEffectiveToDateInFuture()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard1", "ST0001", 101, "1.1", DateTime.Today.AddDays(1))
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.1", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(2)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_ExcludesOrganisations_WhenStandardEffectiveToDateInPast()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard1", "ST0001", 101, "1.1", DateTime.Today.AddDays(-1))
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.1", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001")
                        .VerifyResultsNotContain("EPA0002");
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesStandards_WhenOrganisationStandardHasAtLeastOneStandardVersion()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_ExcludesStandards_WhenOrganisationDoesNotHaveAtLeastOneStandardVersion()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister) // no standard version
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom.AddDays(5), DefaultDateStandardApprovedOnRegister.AddDays(5)); // the later standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesStandards_WhenOrganisationStandardVersionHasEffectiveToDateNotSet()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesStandards_WhenOrganisationStandardVersionHasEffectiveToDateInFuture()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, DateTime.Today.AddDays(10))
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_ExcludesStandards_WhenOrganisationStandardVersionHasEffectiveToDateInPast()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, DateTime.Today.AddDays(-10))
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom.AddDays(5), DefaultDateStandardApprovedOnRegister.AddDays(5)); // the later standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesStandards_WhenOrganisationStandardHasEffectiveToDateNotSet()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesStandards_WhenOrganisationStandardHasEffectiveToDateInFuture()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, DateTime.Today.AddDays(10), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_ExcludesStandards_WhenOrganisationStandardHasEffectiveToDateInPast()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, DateTime.Today.AddDays(-10), DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom.AddDays(5), DefaultDateStandardApprovedOnRegister.AddDays(5)); // the later standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesStandards_WhenStandardHasEffectiveToDateNotSet()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_IncludesStandards_WhenStandardHasEffectiveToDateInFuture()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", DateTime.Today.AddDays(10))
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain("EPA0001", DefaultEffectiveFrom, DefaultDateStandardApprovedOnRegister); // the earlier standard is chosen
                }
            }
        }

        [Test]
        public async Task GetAllOrganisationsWithActiveStandards_ExcludesStandards_WhenStandardHasEffectiveToDateInPast()
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", DateTime.Today.AddDays(-10))
                .WithStandard("TestStandard2", "ST0002", 102, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", DefaultEffectiveFrom.AddDays(5), null, DefaultDateStandardApprovedOnRegister.AddDays(5))
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards();

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
        public async Task GetAllOrganisationsWithActiveStandards_IncludesOrganisationForSpecificUkprn_WhenOrganisationHasAtLeastOneStandardVersion(int ukprn, string expectedEndPointAssessorOrganisationId)
        {
            using (var fixture = new GetAllOrganisationsWithActiveStandardsTestsFixture()
                .WithStandard("TestStandard1", "ST0001", 101, "1.0", null)
                .WithOrganisation("Organisation1", "EPA0001", 123456)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation2", "EPA0002", 612345)
                .WithOrganisationStandard(2, "EPA0002", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0002", 101, "ST0001", "1.0", DefaultEffectiveFrom, null)
                .WithOrganisation("Organisation3", "EPA0003", 561234)
                .WithOrganisationStandard(3, "EPA0003", 101, "ST0001", DefaultEffectiveFrom, null, DefaultDateStandardApprovedOnRegister)
                .WithOrganisationStandardVersion("EPA0003", 101, "ST0001", "1.0", DefaultEffectiveFrom, null))
            {
                var result = await fixture.GetAllOrganisationsWithActiveStandards(ukprn);

                using (new AssertionScope())
                {
                    result.VerifyNumberOfResults(1)
                        .VerifyResultsContain(expectedEndPointAssessorOrganisationId);
                }
            }
        }

        private class GetAllOrganisationsWithActiveStandardsTestsFixture : FixtureBase
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private RegisterQueryRepository _repository;
            public IEnumerable<AssessmentOrganisationListSummary> _results;

            public GetAllOrganisationsWithActiveStandardsTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
                _repository = new RegisterQueryRepository(new UnitOfWork(_sqlConnection));
            }

            public async Task<GetAllOrganisationsWithActiveStandardsTestsFixture> GetAllOrganisationsWithActiveStandards()
            {
                _results = await _repository.GetAssessmentOrganisationsList(null);
                return this;
            }

            public async Task<GetAllOrganisationsWithActiveStandardsTestsFixture> GetAllOrganisationsWithActiveStandards(int ukprn)
            {
                _results = await _repository.GetAssessmentOrganisationsList(ukprn);
                return this;
            }

            public GetAllOrganisationsWithActiveStandardsTestsFixture VerifyNumberOfResults(int numberOfResults)
            {
                _results.Count().Should().Be(numberOfResults);
                return this;
            }

            public GetAllOrganisationsWithActiveStandardsTestsFixture VerifyResultsNotContain(string endPointAssessorOrganisationId)
            {
                _results.Should().NotContain(o => o.Id == endPointAssessorOrganisationId);
                return this;
            }

            public GetAllOrganisationsWithActiveStandardsTestsFixture VerifyResultsContain(string endPointAssessorOrganisationId)
            {
                _results.Should().Contain(o => o.Id == endPointAssessorOrganisationId);
                return this;
            }

            public GetAllOrganisationsWithActiveStandardsTestsFixture VerifyResultsContain(string endPointAssessorOrganisationId, DateTime? earlistStandardEffectiveFromDate, DateTime? earliestDateStandardApprovedOnRegister)
            {
                _results.Should().Contain(o => o.Id == endPointAssessorOrganisationId
                    && o.EarliestEffectiveFromDate == earlistStandardEffectiveFromDate
                    && o.EarliestDateStandardApprovedOnRegister == earliestDateStandardApprovedOnRegister);
                return this;
            }

            public GetAllOrganisationsWithActiveStandardsTestsFixture WithOrganisation(string endPointAssessorName, string endPointAssessorOrganisationId, int ukprn)
            {
                AddOrganisation(endPointAssessorName, endPointAssessorOrganisationId, ukprn);
                return this;
            }

            public GetAllOrganisationsWithActiveStandardsTestsFixture WithStandard(string title, string referenceNumber, int larsCode, string version, DateTime? effectiveTo)
            {
                AddStandard(title, referenceNumber, larsCode, version, effectiveTo);
                return this;
            }

            public GetAllOrganisationsWithActiveStandardsTestsFixture WithOrganisationStandard(int id, string endPointAssessorOrganisationId, int larsCode, string standardReference, 
                DateTime? effectiveFrom, DateTime? effectiveTo, DateTime? dateStandardApprovedOnRegister)
            {
                AddOrganisationStandard(id, endPointAssessorOrganisationId, larsCode, standardReference, effectiveFrom, effectiveTo, dateStandardApprovedOnRegister);
                return this;
            }

            public GetAllOrganisationsWithActiveStandardsTestsFixture WithOrganisationStandardVersion(string endPointAssessorOrganisationId, int larsCode, string standardReference, string version, 
                DateTime? effectiveFrom, DateTime? effectiveTo)
            {
                AddOrganisationStandardVersion(endPointAssessorOrganisationId, larsCode, standardReference, version, effectiveFrom, effectiveTo);
                return this;
            }
        }
    }
}