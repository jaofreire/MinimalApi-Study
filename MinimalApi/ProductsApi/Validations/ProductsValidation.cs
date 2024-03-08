using FluentValidation;
using ModelsLibrary;

namespace ProductsApi.Validations
{
    public class ProductsValidation : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<ProductModel>>();
            var newProduct = context.GetArgument<ProductModel>(0);

            var validation = await validator.ValidateAsync(newProduct);
            if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

            return await next(context);
        }
    }
}
