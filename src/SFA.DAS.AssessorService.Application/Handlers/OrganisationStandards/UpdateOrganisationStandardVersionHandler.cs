using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationStandards
{
    public class UpdateOrganisationStandardVersionHandler : IRequestHandler<UpdateOrganisationStandardVersionRequest, OrganisationStandardVersion>
    {
        private readonly IEpaOrganisationValidator _validator;
        private readonly IOrganisationStandardRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrganisationStandardVersionHandler(IEpaOrganisationValidator validator, IOrganisationStandardRepository repository, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganisationStandardVersion> Handle(UpdateOrganisationStandardVersionRequest request, CancellationToken cancellationToken)
        {
            var validationResponse = await _validator.ValidatorUpdateOrganisationStandardVersionRequest(request);

            if (!validationResponse.IsValid)
            {
                var message = validationResponse.Errors.Aggregate(string.Empty, (current, error) => current + error.ErrorMessage + "; ");

                if (validationResponse.Errors.Any(x => x.StatusCode == ValidationStatusCode.BadRequest.ToString()))
                {
                    throw new BadRequestException(message);
                }
                throw new Exception();
            }

            try
            {
                _unitOfWork.Begin();

                var orgStandardVersion = await _repository.GetOrganisationStandardVersionByOrganisationStandardIdAndVersion(request.OrganisationStandardId, request.OrganisationStandardVersion);
                
                orgStandardVersion.EffectiveFrom = request.EffectiveFrom;
                orgStandardVersion.EffectiveTo = request.EffectiveTo;
                
                await _repository.UpdateOrganisationStandardVersion(orgStandardVersion);

                _unitOfWork.Commit();

                return (OrganisationStandardVersion)orgStandardVersion;
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
