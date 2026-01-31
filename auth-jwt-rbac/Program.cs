using auth_jwt_rbac.Database;
using auth_jwt_rbac.Interfaces;
using auth_jwt_rbac.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(u => u.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    $"Host={Environment.GetEnvironmentVariable("PGHOST")};" +
        $"Port={Environment.GetEnvironmentVariable("PGPORT")};" +
        $"Database={Environment.GetEnvironmentVariable("PGDATABASE")};" +
        $"Username={Environment.GetEnvironmentVariable("PGUSER")};" +
        $"Password={Environment.GetEnvironmentVariable("PGPASSWORD")};" +
        "SSL Mode=Require;Trust Server Certificate=true;"));

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetValue<string>("AppSettings:Issuer")!,
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetValue<string>("AppSettings:Audience")!,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AppSettings:Token")!)),
        ValidateIssuerSigningKey = true,
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
