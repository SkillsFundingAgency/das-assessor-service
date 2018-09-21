using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Setup;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.Apply.Config
{
    public class CreateSequenceRequestHandler : IRequestHandler<CreateSequenceRequest, Sequence>
    {
        private readonly IApplyRepository _applyRepository;

        public CreateSequenceRequestHandler(IApplyRepository applyRepository)
        {
            _applyRepository = applyRepository;
        }
        
        public async Task<Sequence> Handle(CreateSequenceRequest request, CancellationToken cancellationToken)
        {
            var sequences = await _applyRepository.GetWorkflowDefinition();
         
            if (request.Sequence.Order == null)
            {
                SetOrderToMaxPlusOne(request, sequences);
            }
            else
            {
                ReorderSequencesToMakeRoomForNewOne(request, sequences);
            }
            
            sequences.Insert(request.Sequence.Order.Value, request.Sequence);
            var workflowJson = JsonConvert.SerializeObject(sequences);

            _applyRepository.UpdateWorkflowDefinition(workflowJson);

            return request.Sequence;
        }

        private static void ReorderSequencesToMakeRoomForNewOne(CreateSequenceRequest request, List<Sequence> sequences)
        {
            sequences.Where(s => s.Order >= request.Sequence.Order).ToList().ForEach(s => s.Order++);
        }

        private static void SetOrderToMaxPlusOne(CreateSequenceRequest request, List<Sequence> sequences)
        {
            request.Sequence.Order = sequences.Max(s => s.Order).Value + 1;
        }
    }
}