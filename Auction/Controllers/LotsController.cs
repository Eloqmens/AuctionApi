using Application.Commands.Lot.Create;
using Application.Commands.Lot.Delete;
using Application.Commands.Lot.Update;
using Application.Interfaces;
using Application.Queries.Lot.Get;
using Application.Queries.Lot.GetAll;
using Auction.DTO;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public LotsController(IMediator mediator, IMapper mapper)
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
        public async Task<IActionResult> UpdateLot([FromBody] UpdateLotCommandDto updateLotCommandDto)
        {

            var command = _mapper.Map<UpdateLotCommand>(updateLotCommandDto);
            command.UserId = UserId;

            await _mediator.Send(command);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLot(int id)
        {
            var command = new DeleteLotCommand { Id = id, UserId = UserId };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
