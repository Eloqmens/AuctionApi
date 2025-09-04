using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private readonly ICurrentUserService _currentUserService;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId ?? "Anonymous";
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Начинается обработка запроса {RequestName} для пользователя {UserId}", 
                requestName, userId);

            try
            {
                var response = await next();
                stopwatch.Stop();

                _logger.LogInformation("Успешно обработан запрос {RequestName} для пользователя {UserId} за {ElapsedMilliseconds}ms", 
                    requestName, userId, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                _logger.LogError(ex, "Ошибка при обработке запроса {RequestName} для пользователя {UserId} после {ElapsedMilliseconds}ms. Запрос: {Request}", 
                    requestName, userId, stopwatch.ElapsedMilliseconds, JsonSerializer.Serialize(request));

                throw;
            }
        }
    }
}
