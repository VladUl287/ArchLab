using FastEndpoints;
using ProductApi.DTOs.Requests;
using ProductApi.DTOs.Responses;

namespace ProductApi.Endpoints.Commands;

public class CreateProductEndpoint : Endpoint<CreateProductRequest, ProductResponse>
{
    public override void Configure()
    {
        Post("api/product/create");
        AllowAnonymous();
    }

    public override Task<ProductResponse> ExecuteAsync(CreateProductRequest req, CancellationToken ct)
    {
        return base.ExecuteAsync(req, ct);
    }
}
