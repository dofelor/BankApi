using FluentValidation.AspNetCore;
using BankApi.Data;
using BankApi.Infrastructure.Exceptions;
using BankApi.Infrastructure.Validators;
using BankApi.Mappings;
using BankApi.Services;
using BankApi.Services.Generators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BankApi
{
    public class Program
    {
        public static void Main(string[] args)
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
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            builder.Services.AddSingleton<ICardNumberGenerator, CardNumberGenerator>();
            builder.Services.AddSingleton<IAccountNumberGenerator, AccountNumberGenerator>();

            builder.Services.AddScoped <IClientService, ClientService>();
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
            // Включаем Swagger UI всегда, чтобы он был доступен и в контейнере.
            app.UseSwagger();
            app.UseSwaggerUI();



            app.MapControllers();

            // Применяем миграции при старте, чтобы создавались таблицы в новой БД
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            app.Run();
        }
    }
}
