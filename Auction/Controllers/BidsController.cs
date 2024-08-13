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
    public class BidsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public BidsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        private string UserId
        {
            get
            {
                if (HttpContext == null || HttpContext.User == null)
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return string.Empty;
                }

                var userId = HttpContext.User.FindFirst("sub")?.Value
                             ?? HttpContext.User.FindFirst("jti")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User identifier (sub or jti) is null or empty.");
                    return string.Empty;
                }

                return userId;
            }
        }

        [Authorize]
        [HttpPost("{lotId}")]
        public async Task<IActionResult> PlaceBid([FromBody] PlaceBidCommandDto placeBidCommandDto)
        {
            var command = _mapper.Map<PlaceBidCommand>(placeBidCommandDto);
            command.UserId = UserId;

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
