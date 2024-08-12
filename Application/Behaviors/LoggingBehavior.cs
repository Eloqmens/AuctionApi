using Application.Commands.Lot.Create;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Behaviors
{
    public class LoggingBehavior <TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggingBehavior(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (request is CreateLotCommand createLotCommand)
            {
                createLotCommand.UserId = userId;
            }

            return await next();
        }
    }
}
