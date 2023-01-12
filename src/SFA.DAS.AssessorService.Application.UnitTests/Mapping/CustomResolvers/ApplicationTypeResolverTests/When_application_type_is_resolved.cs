using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Mapping.CustomResolvers;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AssessorService.Application.UnitTests.Mapping.CustomResolvers.ApplicationTypeResolverTests
{
    public class When_application_type_is_resolved
    {
        ApplicationTypeResolver Sut = new ApplicationTypeResolver();

        ApplySummary InitialWithFinancialHealthChecks;
        ApplySummary InitialWithoutFinancialHealthChecks;
        ApplySummary AdditionalStandardWithFinancialHealthChecks;
        ApplySummary AdditionalStandardWithoutFinancialHealthChecks;
        ApplySummary OrganisationWithdrawal;
        ApplySummary StandardWithdrawal;

        [SetUp]
        public void Arrange()
        {
            InitialWithFinancialHealthChecks = GetApplySummary(GetSequences(
                    (
                        notRequired: false,
                        ApplyConst.ORGANISATION_SEQUENCE_NO,
                        GetSections(
                            (notRequired: false, ApplyConst.ORGANISATION_DETAILS_SECTION_NO),
                            (notRequired: false, ApplyConst.DECLARATIONS_SECTION_NO),
                            (notRequired: false, ApplyConst.FINANCIAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: false,
                        ApplyConst.STANDARD_SEQUENCE_NO,
                        GetSections(
                            (notRequired: false, ApplyConst.STANDARD_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: false,
                        ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.ORGANISATION_WITHDRAWAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: false,
                        ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.STANDARD_WITHDRAWAL_DETAILS_SECTION_NO))
                    )));

            InitialWithoutFinancialHealthChecks = GetApplySummary(GetSequences(
                    (
                        notRequired: false,
                        ApplyConst.ORGANISATION_SEQUENCE_NO,
                        GetSections(
                            (notRequired: false, ApplyConst.ORGANISATION_DETAILS_SECTION_NO),
                            (notRequired: false, ApplyConst.DECLARATIONS_SECTION_NO),
                            (notRequired: true, ApplyConst.FINANCIAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: false,
                        ApplyConst.STANDARD_SEQUENCE_NO,
                        GetSections(
                            (notRequired: false, ApplyConst.STANDARD_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: true,
                        ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.ORGANISATION_WITHDRAWAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: true,
                        ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.STANDARD_WITHDRAWAL_DETAILS_SECTION_NO))
                    )));

            AdditionalStandardWithFinancialHealthChecks = GetApplySummary(GetSequences(
                    (
                        notRequired: false,
                        ApplyConst.ORGANISATION_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.ORGANISATION_DETAILS_SECTION_NO),
                            (notRequired: true, ApplyConst.DECLARATIONS_SECTION_NO),
                            (notRequired: false, ApplyConst.FINANCIAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: false,
                        ApplyConst.STANDARD_SEQUENCE_NO,
                        GetSections(
                            (notRequired: false, ApplyConst.STANDARD_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: true,
                        ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.ORGANISATION_WITHDRAWAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: true,
                        ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.STANDARD_WITHDRAWAL_DETAILS_SECTION_NO))
                    )));

            AdditionalStandardWithoutFinancialHealthChecks = GetApplySummary(GetSequences(
                    (
                        notRequired: false,
                        ApplyConst.ORGANISATION_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.ORGANISATION_DETAILS_SECTION_NO),
                            (notRequired: true, ApplyConst.DECLARATIONS_SECTION_NO),
                            (notRequired: true, ApplyConst.FINANCIAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: false,
                        ApplyConst.STANDARD_SEQUENCE_NO,
                        GetSections(
                            (notRequired: false, ApplyConst.STANDARD_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: true,
                        ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.ORGANISATION_WITHDRAWAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: true,
                        ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.STANDARD_WITHDRAWAL_DETAILS_SECTION_NO))
                    )));

            OrganisationWithdrawal = GetApplySummary(GetSequences(
                    (
                        notRequired: true,
                        ApplyConst.ORGANISATION_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.ORGANISATION_DETAILS_SECTION_NO),
                            (notRequired: true, ApplyConst.DECLARATIONS_SECTION_NO),
                            (notRequired: true, ApplyConst.FINANCIAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: true,
                        ApplyConst.STANDARD_SEQUENCE_NO,
                        GetSections(
                            (notRequired: false, ApplyConst.STANDARD_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: false,
                        ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: false, ApplyConst.ORGANISATION_WITHDRAWAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: true,
                        ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.STANDARD_WITHDRAWAL_DETAILS_SECTION_NO))
                    )));

            StandardWithdrawal = GetApplySummary(GetSequences(
                    (
                        notRequired: true,
                        ApplyConst.ORGANISATION_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.ORGANISATION_DETAILS_SECTION_NO),
                            (notRequired: true, ApplyConst.DECLARATIONS_SECTION_NO),
                            (notRequired: true, ApplyConst.FINANCIAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: true,
                        ApplyConst.STANDARD_SEQUENCE_NO,
                        GetSections(
                            (notRequired: false, ApplyConst.STANDARD_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: true,
                        ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: true, ApplyConst.ORGANISATION_WITHDRAWAL_DETAILS_SECTION_NO))
                    ),
                    (
                        notRequired: false,
                        ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO,
                        GetSections(
                            (notRequired: false, ApplyConst.STANDARD_WITHDRAWAL_DETAILS_SECTION_NO))
                    )));
        }

        [Test]
        public void Then_InitialWithFinancialHealthChecks_is_resolved_correctly()
        {
            var applicationType = Sut.Resolve(InitialWithFinancialHealthChecks, new ApplicationResponse(), "ApplicationType", default);
            applicationType.Should().Be(ApplicationTypes.InitialWithFinancialHealthChecks);
        }

        [Test]
        public void Then_InitialWithoutFinancialHealthChecks_is_resolved_correctly()
        {
            var applicationType = Sut.Resolve(InitialWithoutFinancialHealthChecks, new ApplicationResponse(), "ApplicationType", default);
            applicationType.Should().Be(ApplicationTypes.InitialWithoutFinancialHealthChecks);
        }

        [Test]
        public void Then_AdditionalStandardWithFinancialHealthChecks_is_resolved_correctly()
        {
            var applicationType = Sut.Resolve(AdditionalStandardWithFinancialHealthChecks, new ApplicationResponse(), "ApplicationType", default);
            applicationType.Should().Be(ApplicationTypes.AdditionalStandardWithFinancialHealthChecks);
        }

        [Test]
        public void Then_AdditionalStandardWithoutFinancialHealthChecks_is_resolved_correctly()
        {
            var applicationType = Sut.Resolve(AdditionalStandardWithoutFinancialHealthChecks, new ApplicationResponse(), "ApplicationType", default);
            applicationType.Should().Be(ApplicationTypes.AdditionalStandardWithoutFinancialHealthChecks);
        }

        [Test]
        public void Then_OrganisationWithdrawal_is_resolved_correctly()
        {
            var applicationType = Sut.Resolve(OrganisationWithdrawal, new ApplicationResponse(), "ApplicationType", default);
            applicationType.Should().Be(ApplicationTypes.OrganisationWithdrawal);
        }

        [Test]
        public void Then_StandardWithdrawal_is_resolved_correctly()
        {
            var applicationType = Sut.Resolve(StandardWithdrawal, new ApplicationResponse(), "ApplicationType", default);
            applicationType.Should().Be(ApplicationTypes.StandardWithdrawal);
        }

        private IEnumerable<ApplyTypes.ApplySection> GetSections(params (bool notRequired, int sectionNo)[] sections)
        {
            foreach (var section in sections)
            {
                yield return new ApplyTypes.ApplySection
                {
                    NotRequired = section.notRequired,
                    SectionNo = section.sectionNo
                };
            }
        }

        private IEnumerable<ApplyTypes.ApplySequence> GetSequences(params (bool notRequired, int sequenceNo, IEnumerable<ApplyTypes.ApplySection> sections)[] sequences)
        {
            foreach (var sequence in sequences)
            {
                yield return new ApplyTypes.ApplySequence
                {
                    NotRequired = sequence.notRequired,
                    SequenceNo = sequence.sequenceNo,
                    Sections = sequence.sections.ToList()
                };
            }
        }

        private ApplySummary GetApplySummary(IEnumerable<ApplyTypes.ApplySequence> applySequences)
        {
            return new ApplySummary
            {
                ApplyData = new ApplyTypes.ApplyData
                {
                    Sequences = applySequences.ToList()
                }
            };
        }
    }
}