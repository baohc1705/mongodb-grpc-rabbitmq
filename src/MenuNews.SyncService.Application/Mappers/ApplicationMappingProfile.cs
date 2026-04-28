using AutoMapper;
using MenuNews.SyncService.Application.Common.Models.ReadModels;
using MenuNews.SyncService.Application.DTOs;
using MenuNews.SyncService.Domain.Entities;
using MenuNews.SyncService.Domain.Enums;

namespace MenuNews.SyncService.Application.Mappers;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Menu, MenuDto>();
        CreateMap<News, NewsDto>();

        // Menu Read Model
        CreateMap<MenuReadModel, Menu>();
        CreateMap<MenuReadModel, MenuDetailDto>()
            .ConstructUsing((src, ctx) => new MenuDetailDto(
                src.Id,
                src.Name,
                src.Slug,
                src.DisplayOrder,
                src.IsActive,
                src.CreatedAt,
                ctx.Mapper.Map<List<NewsEmbeddedDto>>(src.News)
            ));

        // News Read Model
        CreateMap<NewsReadModel, News>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<NewsStatus>(src.Status)));
        CreateMap<NewsReadModel, NewsDetailDto>()
            .ConstructUsing((src, ctx) => new NewsDetailDto(
                src.Id,
                src.Title,
                src.Slug,
                src.Summary,
                src.Content,
                src.Thumbnail,
                src.Status,
                src.PublishedAt,
                src.ViewCount,
                src.IsActive,
                src.CreatedAt,
                ctx.Mapper.Map<List<MenuEmbeddedDto>>(src.Menus)
            ));

        // Embedded Models
        CreateMap<NewsEmbedded, NewsEmbeddedDto>()
            .ConstructUsing(src => new NewsEmbeddedDto(
                src.NewsId,
                src.Title,
                src.Slug,
                src.Summary,
                src.Thumbnail,
                src.Status,
                src.PublishedAt,
                src.ViewCount,
                src.CreatedAt,
                src.DisplayOrder
            ));

        CreateMap<MenuEmbedded, MenuEmbeddedDto>()
            .ConstructUsing(src => new MenuEmbeddedDto(
                src.MenuId,
                src.Name,
                src.Slug,
                src.DisplayOrder,
                src.NmDisplayOrder
            ));
    }
}
