using Application.Behaviors;
using Application.Interfaces;
using Application.Mappings;
using Application.Queries.Lot.GetAll;
using Auction.Services;
using Core.Entities;
using FluentValidation;
using IdentityServer4.Models;
using Infrastructure.Data;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace Auction
{
    public static class ServiceConfiguration
    {         
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Добавление контекстов базы данных
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SQLserverAuction")));

            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("SQLserverIdentity")));

            // Настройка Identity и IdentityServer
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddInMemoryApiScopes(new List<ApiScope> { new ApiScope("api1", "My API") })
                .AddInMemoryClients(new List<Client>
                {
                    new Client
                    {
                        ClientId = "client",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets = { new Secret("secret".Sha256()) },
                        AllowedScopes = { "api1" }
                    }
                })
                .AddDeveloperSigningCredential();

            // Настройка аутентификации с использованием JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });

            // Регистрация зависимостей
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(Program).Assembly,
                typeof(GetLotsQueryHandler).Assembly));

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                cfg.AddProfile(new AssemblyMappingProfile(typeof(AppDbContext).Assembly));
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Поведение MediatR
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Настройка Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auction API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }

        public static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminEmail = "admin@example.com";
            string adminPassword = "Admin@1234";

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
                else
                {
                    throw new ApplicationException("Failed to create admin user.");
                }
            }
        }

        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var identityDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                await identityDbContext.Database.EnsureCreatedAsync();
                await appDbContext.Database.EnsureCreatedAsync();

                var adminRole = new IdentityRole("Admin");
                if (!await roleManager.RoleExistsAsync(adminRole.Name))
                {
                    await roleManager.CreateAsync(adminRole);
                }

                var userRole = new IdentityRole("User");
                if (!await roleManager.RoleExistsAsync(userRole.Name))
                {
                    await roleManager.CreateAsync(userRole);
                }

                if (!appDbContext.Categories.Any())
                {
                    appDbContext.Categories.AddRange(
                        new Category { Name = "Electronics" },
                        new Category { Name = "Books" },
                        new Category { Name = "Fashion" },
                        new Category { Name = "Home & Garden" },
                        new Category { Name = "Sports" }
                    );
                }

                await appDbContext.SaveChangesAsync();
            }
        }
    }
}

