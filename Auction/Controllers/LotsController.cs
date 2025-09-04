using Application.Commands.Lot.Create;
using Application.Commands.Lot.Delete;
using Application.Commands.Lot.Update;
using Application.Queries.Lot.Get;
using Application.Queries.Lot.GetAll;
using Auction.DTO;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public LotsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetLots([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int? categoryId = null, [FromQuery] string? searchTerm = null)
        {
            var query = new GetLotsQuery 
            { 
                PageNumber = pageNumber, 
                PageSize = pageSize, 
                CategoryId = categoryId,
                SearchTerm = searchTerm
            };
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateLot([FromBody] CreateLotCommandDto createLotCommandDto)
        {
            var command = _mapper.Map<CreateLotCommand>(createLotCommandDto);
            command.UserId = UserId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateLot([FromBody] UpdateLotCommandDto updateLotCommandDto)
        {

            var command = _mapper.Map<UpdateLotCommand>(updateLotCommandDto);
            command.UserId = UserId;

            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLot(int id)
        {
            var command = new DeleteLotCommand { Id = id, UserId = UserId };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
