namespace MenuNews.SyncService.Application.DTOs;

public record MenuDto(
    Guid Id, 
    string Name, 
    string Slug,
    int DisplayOrder, 
    bool IsActive, 
    DateTime CreatedAt
);

public record MenuDetailDto(
    Guid Id, 
    string Name, 
    string Slug,
    int DisplayOrder, 
    bool IsActive, 
    DateTime CreatedAt,
    List<NewsEmbeddedDto> News
);

public record NewsEmbeddedDto(
    Guid Id, 
    string Title, 
    string Slug, 
    string Summary,
    string? Thumbnail, 
    string Status,
    DateTime? PublishedAt, 
    int ViewCount,
    DateTime CreatedAt, 
    int DisplayOrder
);