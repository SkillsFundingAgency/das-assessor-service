﻿using System.Collections.Generic;
using MediatR;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        public int UkPrn { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public List<string> Roles { get; set; }
    }

    public class LoginResponse : IRequest
    {
        public string OrganisationName { get; set; }
        public LoginResult Result { get; set; }
    }

    public enum LoginResult
    {
        Valid,
        NotRegistered,
        InvalidRole
    }
}