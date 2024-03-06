using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WebHost.Costumization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("DataBase:ConnectionString");
});

builder.Services.AddServiceSdk(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/carts", async (Cart cart, IDistributedCache redis) =>
{
    await redis.SetStringAsync(cart.UserId, JsonSerializer.Serialize(cart));
    return true;
}).RequireAuthorization("CLIENT");

app.MapGet("/carts/{userId}", async (string userId, IDistributedCache redis) =>
{
    var data = await redis.GetStringAsync(userId);

    if (string.IsNullOrEmpty(data)) return null;

    var carts = JsonSerializer.Deserialize<Cart>(data, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = false
    });

    return carts;

}).RequireAuthorization("CLIENT");

app.UseServiceApplicationAuth();

app.Run();

record Cart (string UserId, List<Products> Products);

record Products (string Name, int Amount, double Price);



