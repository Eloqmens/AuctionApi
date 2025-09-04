using Application.Commands.Bid.PlaceBid;
using Application.Exceptions;
using Application.Interfaces;
using FluentAssertions;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Auction.IntegrationTests.Commands
{
    public class PlaceBidCommandTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly IServiceScope _scope;
        private readonly IMediator _mediator;
        private readonly AppDbContext _context;

        public PlaceBidCommandTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _scope = _factory.Services.CreateScope();
            _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
            _context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        [Fact]
        public async Task PlaceBidCommand_ShouldCreateBid_WhenValidDataProvided()
        {
            // Arrange
            var command = new PlaceBidCommand
            {
                LotId = 1,
                Amount = 150
            };

            var initialBidsCount = _context.Bids.Count();
            var lot = await _context.Lots.FindAsync(1);
            var initialPrice = lot!.CurrentPrice;

            // Act
            await _mediator.Send(command);

            // Assert
            var finalBidsCount = _context.Bids.Count();
            finalBidsCount.Should().Be(initialBidsCount + 1);

            var updatedLot = await _context.Lots.FindAsync(1);
            updatedLot!.CurrentPrice.Should().Be(150);
            updatedLot.CurrentPrice.Should().BeGreaterThan(initialPrice);

            var createdBid = _context.Bids.OrderByDescending(b => b.Id).First();
            createdBid.LotId.Should().Be(command.LotId);
            createdBid.Amount.Should().Be(command.Amount);
            createdBid.UserId.Should().Be("test-user-default");
            createdBid.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task PlaceBidCommand_ShouldThrowException_WhenBidAmountTooLow()
        {
            // Arrange
            var command = new PlaceBidCommand
            {
                LotId = 2,
                Amount = 200 // Меньше текущей цены (250)
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(
                () => _mediator.Send(command));
        }

        [Fact]
        public async Task PlaceBidCommand_ShouldThrowException_WhenLotNotFound()
        {
            // Arrange
            var command = new PlaceBidCommand
            {
                LotId = 999, // Несуществующий лот
                Amount = 300
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _mediator.Send(command));
        }

        [Fact]
        public async Task PlaceBidCommand_ShouldThrowException_WhenUserBidsOnOwnLot()
        {
            // Arrange
            // Настраиваем тестового пользователя как владельца лота
            var testUserService = _scope.ServiceProvider.GetRequiredService<ICurrentUserService>() as TestCurrentUserService;
            testUserService!.UserId = "test-user-1";

            var command = new PlaceBidCommand
            {
                LotId = 1,
                Amount = 200
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(
                () => _mediator.Send(command));
        }

        private void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
