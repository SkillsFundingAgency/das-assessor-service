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
    public class CreateEpaOrganisationHandler : IRequestHandler<CreateEpaOrganisationRequest, string>
    {
        private readonly IRegisterRepository _registerRepository;
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<CreateEpaOrganisationHandler> _logger;
        private readonly IEpaOrganisationIdGenerator _organisationIdGenerator;
        private readonly ISpecialCharacterCleanserService _cleanser;
        private readonly IEpaOrganisationValidator _validator;

        public CreateEpaOrganisationHandler(IRegisterRepository registerRepository, IRegisterQueryRepository registerQueryRepository, IEpaOrganisationIdGenerator orgIdGenerator, ILogger<CreateEpaOrganisationHandler> logger, 
                                            ISpecialCharacterCleanserService cleanser, IEpaOrganisationValidator validator)
        {
            _registerRepository = registerRepository;
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
            _cleanser = cleanser;
            _organisationIdGenerator = orgIdGenerator;
            _validator = validator;
        }

        public async Task<string> Handle(CreateEpaOrganisationRequest request, CancellationToken cancellationToken)
        {
            ProcessRequestFieldsForSpecialCharacters(request);

            ValidationResponse validationResponse = _validator.ValidatorCreateEpaOrganisationRequest(request);

            if (!validationResponse.IsValid)
            {
                var message = validationResponse.Errors.Aggregate(string.Empty, (current, error) => current + error.ErrorMessage + "; ");
                _logger.LogError(message);
                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()))
                {
                    throw new BadRequestException(message);
                }

                //Incase there already an existing organisation then return that , this is part of ON-2084
                var organisationId = await _registerQueryRepository.GetEpaOrgIdByEndPointAssessmentName(request.Name);
                if(!string.IsNullOrEmpty(organisationId))
                    return organisationId;
            }

         
            var newOrganisationId = _organisationIdGenerator.GetNextOrganisationId();
            if (newOrganisationId == string.Empty)
                throw new Exception("A valid organisation Id could not be generated");

            var organisation = MapOrganisationRequestToOrganisation(request, newOrganisationId);
            return await _registerRepository.CreateEpaOrganisation(organisation);
         
        }

        private void ProcessRequestFieldsForSpecialCharacters(CreateEpaOrganisationRequest request)
        {
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
            
            request.OrganisationReferenceType =
                _cleanser.CleanseStringForSpecialCharacters(request.OrganisationReferenceType);
            request.OrganisationReferenceId =
                _cleanser.CleanseStringForSpecialCharacters(request.OrganisationReferenceId);
            request.EndPointAssessmentOrgId =
                _cleanser.CleanseStringForSpecialCharacters(request.EndPointAssessmentOrgId);
        }

        private static EpaOrganisation MapOrganisationRequestToOrganisation(CreateEpaOrganisationRequest request, string newOrganisationId)
        {
            if (!string.IsNullOrWhiteSpace(request.CompanyNumber))
                request.CompanyNumber = request.CompanyNumber.ToUpper();

            if (!string.IsNullOrWhiteSpace(request.CharityNumber))
                request.CharityNumber = request.CharityNumber.ToUpper();

            var organisation = new EpaOrganisation
            {
                Name = request.Name.Trim(),
                OrganisationId = newOrganisationId,
                OrganisationTypeId = request.OrganisationTypeId,
                Ukprn = request.Ukprn,
                Status = string.IsNullOrEmpty(request.Status) ? "New" : request.Status,
                Id = Guid.NewGuid(),
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
                    CompanySummary = request.CompanySummary,
                    CharityNumber = request.CharityNumber,
                    CharitySummary = request.CharitySummary,                    
                    OrganisationReferenceType = request.OrganisationReferenceType,
                    OrganisationReferenceId = request.OrganisationReferenceId,
                    RoATPApproved = request.RoATPApproved,
                    RoEPAOApproved = request.RoEPAOApproved,
                    EndPointAssessmentOrgId = request.EndPointAssessmentOrgId,
                    FinancialGrades = new List<ApplyTypes.FinancialGrade>(),
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