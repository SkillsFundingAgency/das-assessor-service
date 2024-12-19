using AutoMapper;

namespace SFA.DAS.AssessorService.Application.Handlers
{
    public abstract class BaseHandler
    {
        protected readonly IMapper _mapper;

        protected BaseHandler(IMapper mapper)
        {
            _mapper = mapper;
        }
    }
}
