using FluentValidation;
using ModelsLibrary;
using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Validations
{
    public class ProductsListValidation : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<ProductModel>>();
            var productsList = context.GetArgument<List<ProductModel>>(0);

            foreach (var product in productsList)
            {
                var validation = await validator.ValidateAsync(product);
                if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());
            }

            return await next(context);
        }
    }
}
