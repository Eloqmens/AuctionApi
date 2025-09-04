using Application.Commands.Lot.Create;
using FluentAssertions;
using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Auction.IntegrationTests.Commands
{
    public class CreateLotCommandTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly IServiceScope _scope;
        private readonly IMediator _mediator;
        private readonly AppDbContext _context;

        public CreateLotCommandTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _scope = _factory.Services.CreateScope();
            _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
            _context = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        [Fact]
        public async Task CreateLotCommand_ShouldCreateLot_WhenValidDataProvided()
        {
            // Arrange
            var command = new CreateLotCommand
            {
                Title = "Новый тестовый лот",
                Description = "Описание нового тестового лота",
                StartingPrice = 150,
                EndTime = DateTime.UtcNow.AddDays(3),
                CategoryId = 1,
                UserId = "test-user-3"
            };

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.Should().BeGreaterThan(0);

            var createdLot = await _context.Lots.FindAsync(result);
            createdLot.Should().NotBeNull();
            createdLot!.Title.Should().Be(command.Title);
            createdLot.Description.Should().Be(command.Description);
            createdLot.StartingPrice.Should().Be(command.StartingPrice);
            createdLot.CurrentPrice.Should().Be(command.StartingPrice);
            createdLot.CategoryId.Should().Be(command.CategoryId);
            createdLot.UserId.Should().Be(command.UserId);
            createdLot.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task CreateLotCommand_ShouldThrowValidationException_WhenInvalidDataProvided()
        {
            // Arrange
            var command = new CreateLotCommand
            {
                Title = "", // Пустое название
                Description = "Описание",
                StartingPrice = -10, // Отрицательная цена
                EndTime = DateTime.UtcNow.AddDays(-1), // Время в прошлом
                CategoryId = 1, // Верная категория, но другие поля неверные
                UserId = "test-user"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Application.Exceptions.ValidationException>(
                () => _mediator.Send(command));
        }

        private void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
