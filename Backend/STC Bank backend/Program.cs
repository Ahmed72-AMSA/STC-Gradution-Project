// Program.cs (ASP.NET Core)
using BankingSystemAPI.Hubs;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using STC.Data.Context;
using STC.Helpers;
using STC.Services;
using STC.Services.ChatServices;

var builder = WebApplication.CreateBuilder(args);

// Database configuration
builder.Services.AddDbContext<STCSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services configuration

builder.Services.AddControllers();
builder.Services.AddSignalR();


// CORS configuration
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

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Third-party services configuration
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));
builder.Services.AddTransient<ISMSService, SMSService>();
builder.Services.AddTransient<IOTPService, OTPService>();
builder.Services.AddSingleton<SharedDb>();
builder.Services.AddSingleton<OnnxModelService>();
var app = builder.Build();

// Development-only middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty;
    });
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowReactApp");

app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.UseStaticFiles();


// Run the application
app.Run("http://localhost:5001");
