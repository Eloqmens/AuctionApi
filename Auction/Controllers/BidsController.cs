using Application.Commands.Bid.PlaceBid;
using Application.Queries.Bid;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    public class BidsController : Controller
    {
        private readonly IMediator _mediator;

        public BidsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PlaceBid([FromBody] PlaceBidCommand command)
        {
            await _mediator.Send(command);
            return Ok();
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
