using MediatR;
using Microsoft.Extensions.Logging;

namespace MenuNews.SyncService.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        logger.LogInformation($"[START] Handling {requestName}");
        try
        {
            var response = await next();
            logger.LogInformation($"[END] {requestName} handled successfully");
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[ERROR] {requestName} threw an exception");
            throw;
        }
    }
}
