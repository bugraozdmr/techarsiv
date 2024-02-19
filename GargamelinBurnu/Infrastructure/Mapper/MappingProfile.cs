using AutoMapper;
using Entities.Dtos.SubjectDtos;
using Entities.Models;

namespace GargamelinBurnu.Infrastructure.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Subject, CreateSubjectDto>().ReverseMap();
    }
}