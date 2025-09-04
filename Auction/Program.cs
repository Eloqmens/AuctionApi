using Auction;
using Auction.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

ServiceConfiguration.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Глобальная обработка ошибок (должна быть первой)
app.UseMiddleware<GlobalExceptionMiddleware>();

await ServiceConfiguration.InitializeAsync(app.Services);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await ServiceConfiguration.CreateAdminUser(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auction API v1"));
}

app.UseHttpsRedirection();

// Используем CORS политику, настроенную в ServiceConfiguration
app.UseCors();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }