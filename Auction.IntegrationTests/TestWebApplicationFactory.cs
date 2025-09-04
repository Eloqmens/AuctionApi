using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Auction.IntegrationTests
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Удаляем существующие регистрации DbContext
                var descriptorApp = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptorApp != null)
                {
                    services.Remove(descriptorApp);
                }

                var descriptorIdentity = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppIdentityDbContext>));
                if (descriptorIdentity != null)
                {
                    services.Remove(descriptorIdentity);
                }

                // Добавляем In-Memory базы данных для тестов с уникальными именами
                var dbName = Guid.NewGuid().ToString();
                
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestAuctionDb_{dbName}");
                });

                services.AddDbContext<AppIdentityDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"TestIdentityDb_{dbName}");
                });

                // Замещаем CurrentUserService на тестовый
                var currentUserServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ICurrentUserService));
                if (currentUserServiceDescriptor != null)
                {
                    services.Remove(currentUserServiceDescriptor);
                }
                services.AddSingleton<ICurrentUserService, TestCurrentUserService>();

                // Создаем scope для инициализации данных
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var appDb = scopedServices.GetRequiredService<AppDbContext>();
                var identityDb = scopedServices.GetRequiredService<AppIdentityDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<TestWebApplicationFactory<TStartup>>>();

                appDb.Database.EnsureCreated();
                identityDb.Database.EnsureCreated();

                try
                {
                    SeedTestData(appDb);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка при инициализации тестовых данных");
                    throw;
                }
            });

            builder.UseEnvironment("Testing");
        }

        private static void SeedTestData(AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Core.Entities.Category { Id = 1, Name = "Электроника" },
                    new Core.Entities.Category { Id = 2, Name = "Книги" }
                );
            }

            if (!context.Lots.Any())
            {
                context.Lots.AddRange(
                    new Core.Entities.Lot
                    {
                        Id = 1,
                        Title = "Тестовый лот 1",
                        Description = "Описание тестового лота 1",
                        StartingPrice = 100,
                        CurrentPrice = 100,
                        EndTime = DateTime.UtcNow.AddDays(1),
                        CategoryId = 1,
                        UserId = "test-user-1",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }
                    },
                    new Core.Entities.Lot
                    {
                        Id = 2,
                        Title = "Тестовый лот 2",
                        Description = "Описание тестового лота 2",
                        StartingPrice = 200,
                        CurrentPrice = 250,
                        EndTime = DateTime.UtcNow.AddDays(2),
                        CategoryId = 2,
                        UserId = "test-user-2",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        RowVersion = new byte[] { 2, 3, 4, 5, 6, 7, 8, 9 }
                    }
                );
            }

            context.SaveChanges();
        }
    }

    public class TestCurrentUserService : ICurrentUserService
    {
        public string? UserId { get; set; } = "test-user-default";
    }
}
