using Application.Queries.Lot.GetAll;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Auction.IntegrationTests.Queries
{
    public class GetLotsQueryTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly IServiceScope _scope;
        private readonly IMediator _mediator;

        public GetLotsQueryTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _scope = _factory.Services.CreateScope();
            _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task GetLotsQuery_ShouldReturnPaginatedResult_WhenCalled()
        {
            // Arrange
            var query = new GetLotsQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _mediator.Send(query);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().NotBeEmpty();
            result.Items.Should().HaveCountLessThanOrEqualTo(10);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetLotsQuery_ShouldFilterByCategory_WhenCategoryIdProvided()
        {
            // Arrange
            var query = new GetLotsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                CategoryId = 1
            };

            // Act
            var result = await _mediator.Send(query);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().OnlyContain(lot => lot.CategoryId == 1);
        }

        [Fact]
        public async Task GetLotsQuery_ShouldThrowValidationException_WhenInvalidPageNumber()
        {
            // Arrange
            var query = new GetLotsQuery
            {
                PageNumber = 0, // Неверный номер страницы
                PageSize = 10
            };

            // Act & Assert
            await Assert.ThrowsAsync<Application.Exceptions.ValidationException>(
                () => _mediator.Send(query));
        }

        private void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
