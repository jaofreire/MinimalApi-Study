using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ModelsLibrary;

namespace ProductsApi
{
    public static class Routes
    {
        public static void Map(WebApplication app)
        {
            app.MapGroup("/products").MapGroupPublic().WithTags("productsGets");

            app.MapGroup("/products").MapGroupPostPrivate().WithTags("productsPosts");
        }
    }
}
