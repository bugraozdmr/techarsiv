using AutoMapper;
using Entities.Dtos.Comment;
using Entities.Dtos.SubjectDtos;
using Entities.Models;

namespace GargamelinBurnu.Infrastructure.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Subject, CreateSubjectDto>().ReverseMap();
        CreateMap<Comment, CreateCommentDto>().ReverseMap();
        CreateMap<Comment, updateCommentDto>().ReverseMap();
        CreateMap<Subject, UpdateSubjectDto>().ReverseMap();
    }
}