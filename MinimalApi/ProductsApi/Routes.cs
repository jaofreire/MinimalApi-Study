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
            app.MapGroup("/products/public").MapGroupPublic().WithTags("productsPublic");

            app.MapGroup("/products/private").MapGroupPostPrivate().WithTags("productsPrivate").RequireAuthorization("ADM");
        }
    }
}
