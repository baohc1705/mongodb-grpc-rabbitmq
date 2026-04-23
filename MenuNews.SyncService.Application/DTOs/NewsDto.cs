namespace MenuNews.SyncService.Application.DTOs;

public record NewsDto(
    Guid Id, 
    string Title, 
    string Slug,
    string Status, 
    bool IsActive, 
    DateTime CreatedAt
);

public record NewsDetailDto(
    Guid Id, 
    string Title,
    string Slug,
    string Summary, 
    string Content, 
    string? Thumbnail,
    string Status, 
    DateTime? PublishedAt,
    int ViewCount, 
    bool IsActive, 
    DateTime CreatedAt,
    List<MenuEmbeddedDto> Menus
);

public record MenuEmbeddedDto(
    Guid Id, 
    string Name, 
    string Slug,
    int DisplayOrder, 
    int NmDisplayOrder
);