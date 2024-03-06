using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("DataBase:ConnectionString");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/register", async (User newUser, IDistributedCache redis) =>
{
    await redis.SetStringAsync(newUser.Username, JsonSerializer.Serialize(newUser));
    return true;

});

app.MapPost("/connect/token", async (string userName, IConfiguration configuration, IDistributedCache redis) =>
{
    
    var data =  await redis.GetStringAsync(userName);

    if (string.IsNullOrEmpty(data)) throw new Exception("User Not Found");

    var UserConnect = JsonSerializer.Deserialize<User>(data);

    switch (UserConnect.Role)
    {
        case "ADM":
            return Token.GenerateToken(configuration, UserConnect.Role);


        case "CLIENT":
            return Token.GenerateToken(configuration, UserConnect.Role);

    }

    return false;

});

app.UseHttpsRedirection();

app.Run();



record User (string Username, string Password, string Role);

public static class Token
{
    public static object GenerateToken(IConfiguration configuration, string role)
    {
        var key = Encoding.ASCII.GetBytes(configuration["AuthenticateKey:Key"]);

        var tokenConfig = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, role)
            }),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Expires = DateTime.UtcNow.AddHours(3)
        };
        var tokenHandle = new JwtSecurityTokenHandler();
        var token = tokenHandle.CreateToken(tokenConfig);
        var tokenString = tokenHandle.WriteToken(token);

        return new
        {
            token = tokenString
        };
    }

}