using Application.Commands.Wallet.AddFunds;
using Application.Commands.Wallet.AddWallet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : Controller
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("add-funds")]
        public async Task<IActionResult> AddFunds([FromBody] AddFundsCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("add-card")]
        public async Task<IActionResult> AddCard([FromBody] AddCardCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }
    }
}
