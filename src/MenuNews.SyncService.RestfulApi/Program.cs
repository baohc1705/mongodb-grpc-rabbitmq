using MenuNews.SyncService.Application;
using MenuNews.SyncService.Infrastructure;
using MenuNews.SyncService.Infrastructure.Postgres;
using MenuNews.SyncService.RestfulApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Them dependency injection
builder.Services.AddApplicationService();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddInfrastructurePostgresService(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Them middleware xu ly exception

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
