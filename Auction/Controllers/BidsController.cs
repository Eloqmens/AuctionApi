  using Application.Commands.Bid.PlaceBid;
using Application.Queries.Bid;
using Auction.DTO;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<BidsController> _logger;

        public BidsController(IMediator mediator, IMapper mapper, ILogger<BidsController> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }       

        [Authorize]
        [HttpPost("{lotId}")]
        public async Task<IActionResult> PlaceBid(int lotId, [FromBody] PlaceBidCommandDto placeBidCommandDto)
        {
            var command = new PlaceBidCommand
            {
                LotId = lotId,
                Amount = placeBidCommandDto.Amount,
                UserId = UserId
            };

            _logger.LogInformation("PlaceBid request: lotId={LotId}, amount={Amount}, userId={UserId}",
                command.LotId, command.Amount, command.UserId);

            if (string.IsNullOrEmpty(command.UserId))
            {
                return Unauthorized("Пользователь не авторизован");
            }

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{lotId}")]
        public async Task<IActionResult> GetBids(int lotId)
        {
            var query = new GetBidsQuery { LotId = lotId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }   
    }
}
