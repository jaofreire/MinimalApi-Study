using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary;
using ProductsApi.Validations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace ProductsApi
{
    public static class RouteExtension
    {
        public static RouteGroupBuilder MapGroupPublic(this RouteGroupBuilder group)
        {
            group.MapGet("/getAll", async (ApiDbContext db) =>
            {
                var products = await db.Products.ToListAsync();

                return Results.Ok(products);
            });

            group.MapGet("/byId/{id:int}", async (int id, ApiDbContext db) =>
            {
                var products = await db.Products.FindAsync(id) ??
                 throw new Exception("Product not found");

                return Results.Ok(products);
            });

            group.MapGet("/byName/{name}", async (string name, ApiDbContext db) =>
            {
                var products = db.Products.Where(x => x.Name.Contains(name)) ??
                 throw new Exception("Products not founded");

                return Results.Ok(products);
            });

            group.MapGet("/pagination/{pageNumber}/{pageQuantity}", async (int pageNumber, int pageQuantity, ApiDbContext db) =>
            {
                var products = db.Products.Skip((pageNumber * pageQuantity) - pageQuantity).Take(pageQuantity).ToList();

                return Results.Ok(products);
            });

            return group;
        }

        public static RouteGroupBuilder MapGroupPrivate(this RouteGroupBuilder group)
        {
            group.MapPost("/photo", async (IFormFile file, IBlob blob) =>
            {
                var url = await blob.Upload(file);
                return new { url };
            });

            group.MapPost("/register", RegisterNewProduct)
               .AddEndpointFilter<ProductsValidation>();

            group.MapPost("/registerlist", async (List<ProductModel> productsList, ApiDbContext db, IValidator<ProductModel> validator) =>
            {

                var watchBulk = Stopwatch.StartNew();
                await db.BulkInsertAsync(productsList);
                await db.SaveChangesAsync();
                watchBulk.Stop();

                var elapsed = watchBulk.ElapsedMilliseconds;

                Console.WriteLine($"\n Add Bulk : {elapsed} \n");

                return Results.Ok(productsList);
            }).AddEndpointFilter<ProductsListValidation>();

            group.MapDelete("/remove/{id:int}", async (int id, ApiDbContext db) =>
            {
                var product = await db.Products.FindAsync(id) ??
                 throw new Exception("Product not found");

                db.Products.Remove(product);
                await db.SaveChangesAsync();

                return true;
            });

            group.MapDelete("/removeByName/{name}", async (string name, ApiDbContext db) =>
            {
                var product = await db.Products.FirstOrDefaultAsync(x => x.Name == name) ??
                 throw new Exception("Product not found");

                db.Products.Remove(product);
                await db.SaveChangesAsync();

                return true;
            });

            return group;
        }
     
        public static async Task<ProductModel> RegisterNewProduct(ProductModel newProduct, ApiDbContext db, IValidator<ProductModel> validator)
        {
            await db.Products.AddAsync(newProduct);
            await db.SaveChangesAsync();

            return newProduct;
        }
    }
}
