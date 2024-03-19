using Northwind.Shared;

//Section 1 - import namespaces
using Microsoft.AspNetCore.Identity; //IdentityUser
using Microsoft.EntityFrameworkCore; //UseSqlServer, UseSqlite
using Northwind.Mvc.Data;
using Northwind.Common.DataContext.SqlServer;   //ApplicationDbContext

//Section 2 - configure the host web server including services
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)); //UseSqlite
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()   //enable role management
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

string? sqlServerConnection = builder.Configuration
    .GetConnectionString("NorthwindConnection");
if (sqlServerConnection is null)
{
    Console.WriteLine("SQL Server database connection string is missing!");
}
else
{
    builder.Services.AddNorthwindContext(sqlServerConnection);
}

builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(10);
    options.AddPolicy("views", p => p.SetVaryByQuery(""));
});


var app = builder.Build();

// Section 3 - 
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseOutputCache();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}").CacheOutput("views");

app.MapRazorPages();

app.MapGet("/notcached", () => DateTime.Now.ToString());

app.MapGet("/cached", () => DateTime.Now.ToString()).CacheOutput();

//Section 4 - start the host web server listening for HTTP request
app.Run(); //blocking call
