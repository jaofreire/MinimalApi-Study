using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentValidation;
using ModelsLibrary;
using ProductsApi;
using System.Text.RegularExpressions;
using WebHost.Costumization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddServiceDb<ApiDbContext>(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddServiceSdk(builder.Configuration);

builder.Services.AddTransient<IBlob, Blob>();
builder.Services.AddScoped<IValidator<ProductModel>, ProductsValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

Routes.Map(app);

app.UseServiceApplicationAuth();

app.Run();

public interface IBlob
{
   public Task<string> Upload(IFormFile file);
}

public class Blob : IBlob
{
    private readonly IConfiguration _configuration;

    public Blob(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> Upload(IFormFile file)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        string fileName = Path.ChangeExtension(file.Name, ".png");

        var container = new BlobContainerClient(_configuration["Blob:ConnectionString"], _configuration["Blob:ContainerName"]);
        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders 
            { 
             ContentType = "image/png"
            }
        };
        var blobClient = container.GetBlobClient(fileName);
        await blobClient.UploadAsync(stream, options);

        return container.Uri.AbsoluteUri + "/" + fileName;
    }
}

public class ProductsValidator : AbstractValidator<ProductModel>
{
    public ProductsValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("The name field cannot be empty")
            .MinimumLength(3).WithMessage("Minimum lenght is 3 characters")
            .Must(ValidateIfHaveNumber).WithMessage("Name is not valid: The name cannot have numbers");
    }

    public bool ValidateIfHaveNumber(string name)
    {
        if (Regex.IsMatch(name, "[0-9]")) return false;
        return true;
    }
            
}