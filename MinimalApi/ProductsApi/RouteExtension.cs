using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary;
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

        public static RouteGroupBuilder MapGroupPostPrivate(this RouteGroupBuilder group)
        {
            group.MapPost("/photo", async (IFormFile file, IBlob blob) =>
            {
                var url = await blob.Upload(file);
                return new { url };
            });

            group.MapPost("/register", async (ProductModel newProduct, ApiDbContext db, IValidator<ProductModel> validator) =>
            {
                var validation = await validator.ValidateAsync(newProduct);
                if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

                await db.Products.AddAsync(newProduct);
                await db.SaveChangesAsync();

                return Results.Ok(newProduct);
            });

            group.MapPost("/registerlist", async (List<ProductModel> productsList, ApiDbContext db, IValidator<ProductModel> validator) =>
            {
                foreach (var product in productsList)
                {
                    var validation = await validator.ValidateAsync(product);
                    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());
                }

                var watchBulk = Stopwatch.StartNew();
                await db.BulkInsertAsync(productsList);
                await db.SaveChangesAsync();
                watchBulk.Stop();

                var elapsed = watchBulk.ElapsedMilliseconds;

                Console.WriteLine($"\n Add Bulk : {elapsed} \n");

                return Results.Ok(productsList);
            });

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
    }
}
