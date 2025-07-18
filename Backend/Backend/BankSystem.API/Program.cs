using BankSystem.API.Extension;
using BankSystem.Data.Contexts;
using BankSystem.Data.Entities.Helpers;
using BankSystem.Repository.Repositories;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;
using BankSystem.Service.Services;
using BankSystem.Service.Services.AccountService;
using BankSystem.Service.Services.FileScanService;
using BankSystem.Service.Services.Security;
using BankSystem.Service.Services.SubscriptionService;
using BankSystem.Service.Services.TransactionService;
using BankSystem.Service.Services.UserService;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configure DB Context
            builder.Services.AddDbContext<BankingContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // 2. Load Twilio config
            builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));

            // 3. Enable CORS for React frontend (change port if needed)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5174")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // 4. Add SignalR with detailed errors
            builder.Services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });


            // 5. Add custom application services
            builder.Services.AddApplicationServices();

            // 6. Add controller support
            builder.Services.AddControllers();

            // 7. Swagger setup
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IFileScanService, FileScanService>();
            builder.Services.AddScoped<IVirusTotalReportService, VirusTotalReportService>();

            var app = builder.Build();

            // 8. Dev tools: Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 9. Middleware
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseCors("AllowReactApp");

            app.UseAuthorization();

            // 10. Map endpoints
            app.MapControllers();
            app.MapHub<ChatHub>("/chat");

            app.Run();
        }
    }
}
