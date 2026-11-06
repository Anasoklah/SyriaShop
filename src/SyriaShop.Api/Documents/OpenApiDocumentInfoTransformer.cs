using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SyriaShop.Api.Documents;

public sealed class OpenApiDocumentInfoTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Info.Title = "SyriaShop API";
        document.Info.Description = "E-commerce REST API: catalog, cart, orders. Built with Clean Architecture on .NET 10.";
        document.Info.Version = "1.0";
        document.Info.Contact = new OpenApiContact
        {
            Name = "SyriaShop",
            Url = new Uri("https://github.com/Anasoklah/SyriaShop")
        };

        return Task.CompletedTask;
    }
}
