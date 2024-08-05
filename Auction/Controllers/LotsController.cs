using Application.Commands.Lot.Create;
using Application.Commands.Lot.Delete;
using Application.Commands.Lot.Update;
using Application.Queries.Lot.Get;
using Application.Queries.Lot.GetAll;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotsController : Controller
    {
        private readonly IMediator _mediator;

        public LotsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetLots()
        {
            var query = new GetLotsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLotById(int id)
        {
            var query = new GetLotByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateLot([FromBody] CreateLotCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLot(int id, [FromBody] UpdateLotCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID in the URL does not match ID in the command.");
            }

            await _mediator.Send(command);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLot(int id)
        {
            var command = new DeleteLotCommand { Id = id };
            await _mediator.Send(command);
            return Ok();
        }
    }
}
