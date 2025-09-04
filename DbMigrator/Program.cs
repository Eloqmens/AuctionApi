using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("SQLserver")));
            })
            .Build();

        await MigrateDatabaseAsync(host.Services);
        
        Console.WriteLine("Миграции базы данных успешно применены!");
        Console.WriteLine("Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    private static async Task MigrateDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        Console.WriteLine("Применение миграций базы данных...");
        
        try
        {
            await context.Database.MigrateAsync();
            Console.WriteLine("Миграции успешно применены!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при применении миграций: {ex.Message}");
            throw;
        }
    }
}
