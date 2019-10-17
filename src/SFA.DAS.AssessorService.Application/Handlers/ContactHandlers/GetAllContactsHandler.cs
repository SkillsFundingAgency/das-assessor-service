using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ContactHandlers
{
    public class GetAllContactsHandler : IRequestHandler<GetAllContactsRequest, List<ContactResponse>>
    {
        private readonly IContactQueryRepository _contactQueryRepository;

        public GetAllContactsHandler(IContactQueryRepository contactQueryRepository)
        {
            _contactQueryRepository = contactQueryRepository;
        }

        public async Task<List<ContactResponse>> Handle(GetAllContactsRequest request,
            CancellationToken cancellationToken)
        {
            var response = new List<ContactResponse>();
            var results = await _contactQueryRepository.GetAllContacts(request.EndPointAssessorOrganisationId, request.WithUser);

            if (results == null)
                return response;

            return Mapper
                .Map<List<ContactResponse>>(results);
        }
    }
}
