using MMLib.SwaggerForOcelot.DependencyInjection;
using Newtonsoft.Json.Linq;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Configuration.AddOcelotWithSwaggerSupport(o =>
{
    o.Folder = "Route";
});
builder.Services.AddSwaggerGen();

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerForOcelotUI(o =>
    {
        o.PathToSwaggerGenerator = "/swagger/docs";
        o.ReConfigureUpstreamSwaggerJson = AlterUpStreamSwaggerJson;
    }).UseOcelot().Wait();
}

string AlterUpStreamSwaggerJson(HttpContext context, string arg2)
{
    var swagger = JObject.Parse(arg2);
    return swagger.ToString(Newtonsoft.Json.Formatting.Indented);
}

app.UseHttpsRedirection();



app.Run();

