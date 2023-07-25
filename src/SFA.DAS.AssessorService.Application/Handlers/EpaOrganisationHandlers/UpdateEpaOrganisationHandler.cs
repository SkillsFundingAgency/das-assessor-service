using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.EpaOrganisationHandlers
{
    public class UpdateEpaOrganisationHandler : IRequestHandler<UpdateEpaOrganisationRequest, string>
    {
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly IRegisterRepository _registerRepository;
        private readonly ILogger<UpdateEpaOrganisationHandler> _logger;
        private readonly ISpecialCharacterCleanserService _cleanser;
        private readonly IEpaOrganisationValidator _validator;
        
        public UpdateEpaOrganisationHandler(IRegisterQueryRepository registerQueryRepository, IRegisterRepository registerRepository,  ILogger<UpdateEpaOrganisationHandler> logger, ISpecialCharacterCleanserService cleanser,
            IEpaOrganisationValidator validator)
        {
            _registerQueryRepository = registerQueryRepository;
            _registerRepository = registerRepository;
            _logger = logger;
            _cleanser = cleanser;
            _validator = validator;
        }

        public async Task<string> Handle(UpdateEpaOrganisationRequest request, CancellationToken cancellationToken)
        {
            ProcessRequestFieldsForSpecialCharacters(request);

            ValidationResponse validationResponse = _validator.ValidatorUpdateEpaOrganisationRequest(request);

            if (!validationResponse.IsValid)
            {
                var message = validationResponse.Errors.Aggregate(string.Empty, (current, error) => current + error.ErrorMessage + "; ");
                _logger.LogError(message);
                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()))
                {
                    throw new BadRequestException(message);
                }
            }

            var existingOrganisation = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(request.OrganisationId);

            var organisation = MapOrganisationRequestToOrganisation(request, existingOrganisation);

           return await _registerRepository.UpdateEpaOrganisation(organisation);
        }

        private void ProcessRequestFieldsForSpecialCharacters(UpdateEpaOrganisationRequest request)
        {       
            request.OrganisationId = _cleanser.CleanseStringForSpecialCharacters(request.OrganisationId);  
            request.Name = _cleanser.CleanseStringForSpecialCharacters(request.Name);  
            request.LegalName = _cleanser.CleanseStringForSpecialCharacters(request.LegalName);
            request.TradingName = _cleanser.CleanseStringForSpecialCharacters(request.TradingName);
            request.ProviderName = _cleanser.CleanseStringForSpecialCharacters(request.ProviderName);
            request.Email = _cleanser.CleanseStringForSpecialCharacters(request.Email);
            request.PhoneNumber = _cleanser.CleanseStringForSpecialCharacters(request.PhoneNumber);
            request.WebsiteLink = _cleanser.CleanseStringForSpecialCharacters(request.WebsiteLink);
            request.Address1 = _cleanser.CleanseStringForSpecialCharacters(request.Address1);
            request.Address2 = _cleanser.CleanseStringForSpecialCharacters(request.Address2);
            request.Address3 = _cleanser.CleanseStringForSpecialCharacters(request.Address3);
            request.Address4 = _cleanser.CleanseStringForSpecialCharacters(request.Address4);
            request.Postcode = _cleanser.CleanseStringForSpecialCharacters(request.Postcode);
            request.CompanyNumber = _cleanser.CleanseStringForSpecialCharacters(request.CompanyNumber);
            request.CharityNumber = _cleanser.CleanseStringForSpecialCharacters(request.CharityNumber);
        }

        private static EpaOrganisation MapOrganisationRequestToOrganisation(UpdateEpaOrganisationRequest request, EpaOrganisation existingOrganisation)
        {
            var roEPAOApproved = existingOrganisation?.OrganisationData?.RoEPAOApproved ?? false;

            var status = request.Status ?? existingOrganisation?.Status;
            if (status == "New" && request.ActionChoice == "MakeLive")
            {
                status = "Live";
                roEPAOApproved = true;
            }
            else if (status == "New" && request.ActionChoice == "ApproveApplication")
            {
                status = "New";
                roEPAOApproved = true;
            }
            else if(status == "Applying" && request.ActionChoice == "ApproveApplication")
            {
                roEPAOApproved = true;
            }

            if (!string.IsNullOrWhiteSpace(request.CompanyNumber))
            {
                request.CompanyNumber = request.CompanyNumber.ToUpper();
            }

            if (!string.IsNullOrWhiteSpace(request.CharityNumber))
            {
                request.CharityNumber = request.CharityNumber.ToUpper();
            }

            var organisation = new EpaOrganisation
            {
                Id = existingOrganisation?.Id ?? Guid.Empty,
                Name = request.Name.Trim(),
                OrganisationId = request.OrganisationId.Trim(),
                OrganisationTypeId = request.OrganisationTypeId,
                Ukprn = request.Ukprn,
                Status = status,
                RecognitionNumber = request.RecognitionNumber ?? existingOrganisation?.RecognitionNumber,
                OrganisationData = new OrganisationData
                {
                    Address1 = request.Address1,
                    Address2 = request.Address2,
                    Address3 = request.Address3,
                    Address4 = request.Address4,
                    Postcode = request.Postcode,
                    LegalName = request.LegalName,
                    TradingName = request.TradingName,
                    ProviderName = request.ProviderName,                   
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    WebsiteLink = request.WebsiteLink,
                    CompanyNumber = request.CompanyNumber,
                    CompanySummary = request.CompanySummary ?? existingOrganisation?.OrganisationData?.CompanySummary,
                    CharityNumber = request.CharityNumber,
                    CharitySummary = request.CharitySummary ?? existingOrganisation?.OrganisationData?.CharitySummary,
                    OrganisationReferenceType = existingOrganisation?.OrganisationData?.OrganisationReferenceType,
                    OrganisationReferenceId = existingOrganisation?.OrganisationData?.OrganisationReferenceId,
                    RoATPApproved = existingOrganisation?.OrganisationData?.RoATPApproved ?? false,
                    RoEPAOApproved = roEPAOApproved,
                    EndPointAssessmentOrgId = existingOrganisation?.OrganisationData?.EndPointAssessmentOrgId,
                    FinancialGrades = existingOrganisation?.OrganisationData?.FinancialGrades ?? new List<ApplyTypes.FinancialGrade>(),
                    FHADetails = new FHADetails
                    {
                        FinancialDueDate = request.FinancialDueDate,
                        FinancialExempt = request.FinancialExempt
                    }
                }                
            };

            return organisation;
        }
    }
}
