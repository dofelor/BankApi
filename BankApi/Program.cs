using System.Text;
using BankApi.Data;
using BankApi.Data.Models;
using BankApi.Infrastructure.Exceptions;
using BankApi.Infrastructure.Validators;
using BankApi.Mappings;
using BankApi.Services;
using BankApi.Services.Generators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BankApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers()
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            builder.Services.AddDbContext<AppDbContext>(opt => 
                opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

            // Identity configuration
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // JWT Authentication configuration
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "BankApi_Super_Secret_Jwt_Security_Key_2026_With_Minimum_32_Chars!";
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BankApi";
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BankApiClient";

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();

            // CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders("X-Total-Count");
                });
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            builder.Services.AddSingleton<ICardNumberGenerator, CardNumberGenerator>();
            builder.Services.AddSingleton<IAccountNumberGenerator, AccountNumberGenerator>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IBankAccountService, BankAccountService>();
            builder.Services.AddScoped<ICardService, CardService>();
            builder.Services.AddScoped<IPhoneService, PhoneService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<ITransactionLogService, TransactionLogService>();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateClientDtoValidator>();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            var app = builder.Build();

            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Seed default admin user
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                    await authService.SeedDefaultAdminAsync();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Failed to seed default admin user.");
                }
            }

            await app.RunAsync();
        }
    }
}
