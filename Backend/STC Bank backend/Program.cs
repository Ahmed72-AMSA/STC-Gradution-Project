using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using STC.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<STCSystemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

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



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowReactApp");

app.MapControllers();
app.Run("http://localhost:5001"); 
