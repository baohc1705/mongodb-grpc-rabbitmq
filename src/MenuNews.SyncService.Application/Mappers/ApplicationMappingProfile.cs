using AutoMapper;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;

namespace MenuNews.SyncService.Application.Mappers;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Menu, MenuDto>();
        CreateMap<News, NewsDto>();
    }
}
