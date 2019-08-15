using System;
using System.Linq;
using System.Text;
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
        private readonly IRegisterRepository _registerRepository;
        private readonly IRegisterQueryRepository _registerQueryRepository;
        private readonly ILogger<UpdateEpaOrganisationHandler> _logger;
        private readonly IAuditLogService _auditLogService;
        private readonly ISpecialCharacterCleanserService _cleanser;
        private readonly IEpaOrganisationValidator _validator;
        
        public UpdateEpaOrganisationHandler(IRegisterRepository registerRepository, IRegisterQueryRepository registerQueryRepository,  
            ILogger<UpdateEpaOrganisationHandler> logger, IAuditLogService auditLogService, 
            ISpecialCharacterCleanserService cleanser, IEpaOrganisationValidator validator)
        {
            _registerRepository = registerRepository;
            _registerQueryRepository = registerQueryRepository;
            _logger = logger;
            _auditLogService = auditLogService;
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

            // get the current organisation, removing the primary contact information which is not updated by this request
            var prevOrganisation = await _registerQueryRepository.GetEpaOrganisationByOrganisationId(request.OrganisationId);
            prevOrganisation.PrimaryContact = null;

            var organisation = MapOrganisationRequestToOrganisation(request);

            var differences = _auditLogService.Compare<EpaOrganisation>(prevOrganisation, organisation);
            differences?.ForEach(p => 
            {
                /*_auditRespository.CreateAuditLog(new AuditLog
                {
                    OrganisationId = "",
                    UpdatedBy,
                    UpdatedAt,
                    AuditData = new AuditData
                    {
                        FieldChanged = p.FieldChanged,
                        PreviousValue = p.PreviousValue,
                        CurrentValue = p.CurrentValue
                    }
                });*/
            });
            
            return await _registerRepository.UpdateEpaOrganisation(organisation);
        }

        private void ProcessRequestFieldsForSpecialCharacters(UpdateEpaOrganisationRequest request)
        {       
            request.OrganisationId = _cleanser.CleanseStringForSpecialCharacters(request.OrganisationId);  
            request.Name = _cleanser.CleanseStringForSpecialCharacters(request.Name);  
            request.LegalName = _cleanser.CleanseStringForSpecialCharacters(request.LegalName);
            request.TradingName = _cleanser.CleanseStringForSpecialCharacters(request.TradingName);
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

        private static EpaOrganisation MapOrganisationRequestToOrganisation(UpdateEpaOrganisationRequest request)
        {
            var status = request.Status;
            if (status == "New" && request.ActionChoice == "MakeLive")
            {
                status = "Live";
            }

            if (!String.IsNullOrWhiteSpace(request.CompanyNumber))
            {
                request.CompanyNumber = request.CompanyNumber.ToUpper();
            }

            var organisation = new EpaOrganisation
            {
                Name = request.Name.Trim(),
                OrganisationId = request.OrganisationId.Trim(),
                OrganisationTypeId = request.OrganisationTypeId,
                Ukprn = request.Ukprn,
                OrganisationData = new OrganisationData
                {
                    Address1 = request.Address1,
                    Address2 = request.Address2,
                    Address3 = request.Address3,
                    Address4 = request.Address4,
                    LegalName = request.LegalName,
                    TradingName = request.TradingName,
                    Postcode = request.Postcode,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    WebsiteLink = request.WebsiteLink,
                    CompanyNumber = request.CompanyNumber,
                    CharityNumber = request.CharityNumber,
                    FHADetails = new FHADetails { 
                    FinancialDueDate = request.FinancialDueDate,
                    FinancialExempt = request.FinancialExempt
                    }
                },
                Status = status
            };

            return organisation;
        }
    }
}
