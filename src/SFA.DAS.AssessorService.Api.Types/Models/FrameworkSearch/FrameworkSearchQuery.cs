using System;
using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch
{
    public class FrameworkSearchQuery : IRequest<List<FrameworkSearchResult>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth{ get; set; }
    }
}