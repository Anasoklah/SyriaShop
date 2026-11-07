using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;
using SyriaShop.Api;
using SyriaShop.Application;
using SyriaShop.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var envFilePath = EnvFileLocator.Locate()
    ?? Path.Combine(builder.Environment.ContentRootPath, ".env");
builder.Configuration.AddInMemoryCollection(EnvFileLoader.Load(envFilePath));

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer(new SyriaShop.Api.Documents.OpenApiDocumentInfoTransformer()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetSection("Auth:Issuer").Value,
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetSection("Auth:Audience").Value,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Auth:SigningKey").Value!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapControllers();

await app.Services.SeedIdentityAsync(builder.Configuration);

app.Run();

public partial class Program;
