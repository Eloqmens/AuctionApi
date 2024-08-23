using Auction;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Регистрация сервисов
ServiceConfiguration.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

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

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();

app.Run();
