using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Consts;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply
{
    public class OrganisationStandardVersionOptInHandler : IRequestHandler<OrganisationStandardVersionOptInRequest, OrganisationStandardVersion>
    {
        private readonly IOrganisationStandardRepository _repository;
        private readonly IApplyRepository _applyRepository;
        private readonly IStandardRepository _standardRepository;
        private readonly IContactQueryRepository _contactQueryRepository;
        private readonly IEMailTemplateQueryRepository _eMailTemplateQueryRepository;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrganisationStandardVersionOptInHandler> _logger;

        public OrganisationStandardVersionOptInHandler(IOrganisationStandardRepository repository, IApplyRepository applyRepository, IStandardRepository standardRepository,
            IContactQueryRepository contactQueryRepository, IEMailTemplateQueryRepository eMailTemplateQueryRepository, IMediator mediator,
            IUnitOfWork unitOfWork, ILogger<OrganisationStandardVersionOptInHandler> logger)
        {
            _repository = repository;
            _applyRepository = applyRepository;
            _standardRepository = standardRepository;
            _contactQueryRepository = contactQueryRepository;
            _eMailTemplateQueryRepository = eMailTemplateQueryRepository;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<OrganisationStandardVersion> Handle(OrganisationStandardVersionOptInRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Remove out of unit of work due to crossover betwen EF, Dapper and UnitOfWorkTransaction Conflict
                // It's also a Read so has no impact in that case.
                var submittingContact = await _contactQueryRepository.GetContactById(request.SubmittingContactId);

                _unitOfWork.Begin();

                var orgStandard = await _repository.GetOrganisationStandardByOrganisationIdAndStandardReference(request.EndPointAssessorOrganisationId, request.StandardReference);

                var entity = new Domain.Entities.OrganisationStandardVersion
                {
                    StandardUId = request.StandardUId,
                    Version = request.Version,
                    OrganisationStandardId = orgStandard.Id,
                    EffectiveFrom = request.EffectiveFrom,
                    EffectiveTo = request.EffectiveTo,
                    DateVersionApproved = request.DateVersionApproved,
                    Comments = request.Comments,
                    Status = request.Status
                };

                var existingVersion = await _repository.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(orgStandard.Id, request.Version);
                if (existingVersion != null)
                    throw new InvalidOperationException("OrganisationStandardVersion already exists");

                await _repository.CreateOrganisationStandardVersion(entity);
                var orgStandardVersion = (OrganisationStandardVersion)entity;

                var application = await _applyRepository.GetApply(request.ApplicationId);
                var standard = await _standardRepository.GetStandardVersionByStandardUId(request.StandardUId);

                application.ApplicationStatus = ApplicationStatus.Approved;
                application.ReviewStatus = ApplicationStatus.Approved;
                application.StandardReference = request.StandardReference;
                application.StandardCode = standard.LarsCode;
                application.ApplyData.Apply.StandardCode = standard.LarsCode;
                application.ApplyData.Apply.StandardReference = request.StandardReference;
                application.ApplyData.Apply.StandardName = standard.Title;
                application.ApplyData.Apply.Versions = new List<string>() { request.Version };

                await _applyRepository.SubmitApplicationSequence(application);

                await NotifyContact(submittingContact, application.ApplyData, cancellationToken);

                _unitOfWork.Commit();
                    
                return orgStandardVersion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to opt-in standard version {request.StandardReference} {request.Version} for Organisation {request.EndPointAssessorOrganisationId}");
                _unitOfWork.Rollback();
                throw;
            }
        }

        private async Task NotifyContact(Domain.Entities.Contact contactToNotify, ApplyData applyData, CancellationToken cancellationToken)
        {
            var email = contactToNotify.Email;
            var contactname = contactToNotify.DisplayName;
            var standard = applyData.Apply.StandardName;
            var standardreference = applyData.Apply.StandardReference;
            var version = applyData.Apply.Versions.First();

            var emailTemplate = await _eMailTemplateQueryRepository.GetEmailTemplate(EmailTemplateNames.ApplyEPAOStandardOptin);
            await _mediator.Send(new SendEmailRequest(email, emailTemplate, new { contactname, standard, standardreference, version }), cancellationToken);
        }
    }
}