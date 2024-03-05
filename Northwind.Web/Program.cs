using Northwind.Common.DataContext.SqlServer;
using Microsoft.AspNetCore.Server.Kestrel.Core; //HttpProtocols

//konfigurerar tjänster
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddNorthwindContext();
builder.Services.AddRequestDecompression();
builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        listenOptions.UseHttps(); //HTTP/3 kräver högre säkerhet
    });
});
var app = builder.Build();

//konfigurerar HTTP-pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRequestDecompression();
app.Use(async (HttpContext context, Func<Task> next) =>
{
    RouteEndpoint? rep = context.GetEndpoint() as RouteEndpoint;

    if (rep is not null)
    {
        Console.WriteLine($"Endpoint name: {rep.DisplayName}");
        Console.WriteLine($"Endpoint route pattern: {rep.RoutePattern.RawText}");
    }

    if (context.Request.Path == "/bonjour")
    {
        await context.Response.WriteAsync("Bonjour Monde!");
        return;
    }
    await next();
});
app.UseDefaultFiles();//index.html, default.html
app.UseStaticFiles();

app.MapRazorPages();
app.MapGet("/hello", () => "Hello World!");

//startar web server
app.Run();

Console.WriteLine("This executes after the web server has stopped!");