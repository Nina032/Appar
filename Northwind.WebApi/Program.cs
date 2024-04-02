using Northwind.Common.DataContext.SqlServer; //AddNorthwindContext
using Microsoft.AspNetCore.Mvc.Formatters; // för att använda IOuputFormatter
using Microsoft.Extensions.Caching.Memory; //för IMemoryCache och andra
using Northwind.WebApi.Repositories; //ICustomerRepository
using Swashbuckle.AspNetCore.SwaggerUI; // För att anvädna SubmitMethod
using Microsoft.AspNetCore.HttpLogging; //För HttpLoggingFields

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096; //Standard är 32k.
    options.ResponseBodyLogLimit = 4096; //Standard är 32k.
});

builder.Services.AddSingleton<IMemoryCache>(
    new MemoryCache(new MemoryCacheOptions()));

builder.Services.AddNorthwindContext();
builder.Services.AddControllers(options =>
{
    Console.WriteLine("Default output formatters:");
    foreach (IOutputFormatter formatter in options.OutputFormatters)
    {
        OutputFormatter? mediaFormatter = formatter as OutputFormatter;
        if (mediaFormatter is null)
        {
            Console.WriteLine($"  {formatter.GetType().Name}");
        }
        else //OutputFormatter klass använder SupportedMediaTypes
        {
            Console.WriteLine("  {0}, Media types: {1}",
                arg0: mediaFormatter.GetType().Name,
                arg1: string.Join(", ", mediaFormatter.SupportedMediaTypes));
        }
    }

})
    .AddXmlDataContractSerializerFormatters()
    .AddXmlSerializerFormatters();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json",
            "Northwind Service API Version 1");
        c.SupportedSubmitMethods(new[]
        {
            SubmitMethod.Get, SubmitMethod.Post,
            SubmitMethod.Put, SubmitMethod.Delete });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
