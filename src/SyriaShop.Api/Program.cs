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
builder.Services.AddExceptionHandler<CatalogExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapControllers();

app.Run();

public partial class Program;
