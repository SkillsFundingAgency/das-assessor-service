﻿using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    public interface IApplicationService
    {
        Task<CreateApplicationRequest> BuildStandardWithdrawalRequest(ContactResponse contact, OrganisationResponse organisation, int standardCode, string referenceFormat);
        Task<CreateApplicationRequest> BuildOrganisationWithdrawalRequest(ContactResponse contact, OrganisationResponse organisation, string referenceFormat);
        Task<CreateApplicationRequest> BuildInitialRequest(ContactResponse contact, OrganisationResponse org, string referenceFormat);
        Task<bool> ResetApplicationToStage1(Guid id);
    }
}
