﻿using AutoMapper;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Entities;


namespace SFA.DAS.AssessorService.Application.Api.AutoMapperProfiles
{
    public class ContactProfile : Profile
    {
        public ContactProfile()
        {
            CreateMap<Contact, ContactResponse>();

            CreateMap<CreateContactRequest, Contact>()
                .ReverseMap();

        }
    }
}