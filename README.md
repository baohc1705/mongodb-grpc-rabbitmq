# MenuNews.SyncService

Service quản lý Menu và News, hỗ trợ đồng bộ dữ liệu giữa SQL Server và MongoDB thông qua RabbitMQ. Được xây dựng theo kiến trúc Clean Architecture và CQRS.

---

## Tổng quan

Hệ thống gồm hai endpoint chính:
- **gRPC** — nhận yêu cầu tạo Menu/News từ các service khác
- **REST API** — phục vụ client đọc/ghi dữ liệu thông thường

Luồng dữ liệu cơ bản:

```
gRPC / REST API
      │
      V
  MediatR (CQRS)
      │
      ├─> SQL Server (Write DB)
      │       └─> OutboxMessage (pending)
      │
      └─> OutboxProcessor (background)
              │
              V
          RabbitMQ
              │
      ┌───────┴────────┐
      V                V
MenuConsumers    NewsUpsertedConsumer
      │                │
      └────────┬───────┘
               V
           MongoDB (Read DB)
```

Outbox Pattern được áp dụng để đảm bảo tính nhất quán giữa việc lưu dữ liệu vào SQL và publish message lên RabbitMQ — tránh trường hợp lưu DB thành công nhưng message bị mất.

---

## Cấu trúc project

```
src/
├── MenuNews.SyncService.Domain          # Entities, Events, Enums
├── MenuNews.SyncService.Application     # CQRS handlers, Interfaces, DTOs
├── MenuNews.SyncService.Infrastructure  # EF Core, MongoDB, RabbitMQ
├── MenuNews.SyncService.Grpc            # gRPC entry point
└── MenuNews.SyncService.RestfulApi      # REST API entry point
```

---

## Tech stack

| Thành phần | Công nghệ |
|---|---|
| Framework | ASP.NET Core 8 |
| Write DB | SQL Server + Entity Framework Core |
| Read DB | MongoDB |
| Message broker | RabbitMQ |
| CQRS | MediatR |
| Object mapping | AutoMapper |
| gRPC | Grpc.AspNetCore |

---

## Yêu cầu môi trường

- .NET 8 SDK
- SQL Server
- MongoDB
- RabbitMQ

---

## Cấu hình

File `appsettings.json` trong project Grpc và RestfulApi:

```json
{
  "ConnectionStrings": {
    "AppDbContext": "Data Source=...;Initial Catalog=MenuNewsSyncDB;..."
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://yourusername:yourpassword@localhost:27017",
    "DatabaseName": "MenuNewsReadDb",
    "MenusCollectionName": "Menus",
    "NewsCollectionName": "News"
  },
  "RabbitMqSettings": {
    "HostName": "localhost",
    "UserName": "yourusername",
    "Password": "yourpassword",
    "DirectExchange": "menu.direct.exchange",
    "MenuQueue": "menu.queue"
  }
}
```

Kestrel mặc định expose 3 port:
- `http://localhost:5000` — HTTP1
- `https://localhost:5001` — HTTP1 + HTTP2
- `http://localhost:5002` — HTTP2 (gRPC only)

---

## Chạy local

```bash
# Chạy gRPC server
dotnet run --project src/MenuNews.SyncService.Grpc

# Chạy REST API
dotnet run --project src/MenuNews.SyncService.RestfulApi
```

---

## Một số điểm đáng chú ý

**Outbox Pattern**

Thay vì publish thẳng lên RabbitMQ sau khi lưu DB, handler sẽ tạo thêm một bản ghi `OutboxMessage` trong cùng một transaction. `OutboxProcessor` (background service) sẽ poll định kỳ mỗi 10 giây, lấy các message có trạng thái `PENDING` và publish lên broker. Sau khi publish xong thì cập nhật trạng thái sang `PROCESSED` hoặc `FAILED` nếu có lỗi.


**Đồng bộ MongoDB**

Sau khi RabbitMQ nhận được event từ OutboxProcessor, các consumer tương ứng (`MenuUpsertedConsumer`, `NewsUpsertedConsumer`,...) sẽ upsert dữ liệu vào MongoDB — phục vụ cho các query read-heavy mà không cần join nhiều bảng.

---
