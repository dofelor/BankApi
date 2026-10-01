
using BankApi.Data;
using BankApi.Infrastructure.Exceptions;
using BankApi.Mappings;
using BankApi.Services;
using BankApi.Services.Generators;
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
            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
            builder.Services.AddSingleton<ICardNumberGenerator, CardNumberGenerator>();
            builder.Services.AddSingleton<IAccountNumberGenerator, AccountNumberGenerator>();
            builder.Services.AddScoped <IClientService, ClientService>();
            builder.Services.AddScoped<IBankAccountService, BankAccountService>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }



            app.MapControllers();

            app.Run();
        }
    }
}
