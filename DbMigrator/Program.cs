using Microsoft.EntityFrameworkCore;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<DbMigratorDbContext>(options =>
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("SQLserver")));
            })
            .Build();

        await MigrateDatabaseAsync(host.Services);
        await host.RunAsync();
    }

    private static async Task MigrateDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbMigratorDbContext>();
        await context.Database.MigrateAsync();
    }
}
