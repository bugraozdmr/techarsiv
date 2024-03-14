using AutoMapper;
using Entities.Dtos.Article;
using Entities.Dtos.Ban;
using Entities.Dtos.Comment;
using Entities.Dtos.Notification;
using Entities.Dtos.Report;
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
        CreateMap<Report, CreateReportDto>().ReverseMap();
        CreateMap<Notification, NotificationDto>().ReverseMap();
        CreateMap<Article, CreateArticleDto>().ReverseMap();
        CreateMap<Article, UpdateArticleDto>().ReverseMap();
        CreateMap<Ban, BanCauseDto>().ReverseMap();
    }
}