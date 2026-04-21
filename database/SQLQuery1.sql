-- NEWS
CREATE TABLE News (
    new_id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    new_title NVARCHAR(255),
    new_slug NVARCHAR(255),
    new_summary NVARCHAR(MAX),
    new_content NVARCHAR(MAX),
    new_thumbnail NVARCHAR(500),
    new_status INT,
    new_published_at DATETIME,
    new_view_count INT,
    new_created_at DATETIME DEFAULT GETDATE(),
    new_updated_at DATETIME,
    new_is_active BIT,
    new_deleted_at DATETIME
);

-- MENUS
CREATE TABLE Menus (
    menu_id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    menu_name NVARCHAR(255),
    menu_slug NVARCHAR(255),
    menu_display_order INT,
    menu_is_active BIT,
    menu_created_at DATETIME DEFAULT GETDATE(),
    menu_updated_at DATETIME,
    menu_deleted_at DATETIME
);

-- NEWS_MENUS (NO FK)
CREATE TABLE NewsMenus (
    nm_id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    nm_news_id UNIQUEIDENTIFIER,
    nm_menu_id UNIQUEIDENTIFIER,
    nm_created_at DATETIME DEFAULT GETDATE(),
    nm_updated_at DATETIME,
    nm_deleted_at DATETIME,
    nm_is_active BIT
);


DECLARE @i INT = 1;
WHILE @i <= 10
BEGIN
    INSERT INTO News (
        new_title, new_slug, new_summary, new_content,
        new_thumbnail, new_status, new_published_at,
        new_view_count, new_is_active
    )
    VALUES (
        CONCAT('News Title ', @i),
        CONCAT('news-title-', @i),
        CONCAT('Summary ', @i),
        CONCAT('Content ', @i),
        CONCAT('thumb', @i, '.jpg'),
        1,
        GETDATE(),
        @i * 10,
        1
    );

    SET @i = @i + 1;
END



SET NOCOUNT ON;
DECLARE @j INT = 1;

WHILE @j <= 10
BEGIN
    INSERT INTO Menus (
        menu_name, menu_slug, menu_display_order,
        menu_is_active
    )
    VALUES (
        CONCAT('Menu ', @j),
        CONCAT('menu-', @j),
        @j,
        1
    );

    SET @j = @j + 1;
END

DECLARE @k INT = 1;

WHILE @k <= 10
BEGIN
    INSERT INTO NewsMenus (
        nm_news_id,
        nm_menu_id,
        nm_is_active
    )
    VALUES (
        (SELECT TOP 1 new_id FROM News ORDER BY NEWID()),
        (SELECT TOP 1 menu_id FROM Menus ORDER BY NEWID()),
        1
    );

    SET @k = @k + 1;
END