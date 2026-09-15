using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ShuttleVNBackend.Api.Middlewares;
using ShuttleVNBackend.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "https://shuttlevn.vercel.app" // assuming so :)
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.Cookie.HttpOnly = true;
        options.Cookie.Name = "shuttlevn.auth";
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>          
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
            options.Events.OnRedirectToAccessDenied = context =>   
        {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"))
    .AddPolicy("StaffOnly", policy => policy.RequireRole("Employee", "Admin"))
    .AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "true"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("Frontend");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<ShuttleVnDbContext>();
db.Database.Migrate();

app.Run();