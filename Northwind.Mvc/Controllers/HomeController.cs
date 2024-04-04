using Microsoft.AspNetCore.Mvc; //Controller, IActionResult
using Northwind.Mvc.Models;//ErrorViewModel
using System.Diagnostics; //Activity
using Northwind.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Northwind.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext db;
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(ILogger<HomeController> logger,
            NorthwindContext injectedContext,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            db = injectedContext;
            _clientFactory = httpClientFactory;
        }

        [ResponseCache(Duration = 10/*seconds*/, 
            Location = ResponseCacheLocation.Any)]

        public async Task<IActionResult> Index()
        {
            _logger.LogError("This is a serious error");
            _logger.LogWarning("This is your first warning!");
            _logger.LogWarning("Second warning");
            _logger.LogInformation("I am in the Index method of the HomeController.");

            HomeIndexViewModel model = new
                (
                    VisitorCount: Random.Shared.Next(1,1001),
                    Categories: await db.Categories.ToListAsync(),
                    Products: await db.Products.ToListAsync()
                );

            return View(model); //pass model to view
        }
        [Route("private")]
        [Authorize(Roles = "Administrators")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> ProductDetail(int? id, string alertstyle ="success")
        {
            ViewData["alertstyle"] = alertstyle;

            if(!id.HasValue)
            {
                return BadRequest("You must pass a product ID in the route, " +
                    "for example, /Home/ProductDetail/13");
            }
            Product? model = await db.Products.SingleOrDefaultAsync(p => p.ProductId == id);
            if(model is null)
            {
                return NotFound($"ProductId {id} not found.");
            }
            return View(model);
        }
        
        public IActionResult ModelBinding()
        {
            return View(); //används för sidan med formulär att skicka
        }
        [HttpPost] //använder detta action metod för POSTs
        public IActionResult ModelBinding(Thing thing)
        {
            HomeModelBindingViewModel model = new(
                Thing: thing, HasErrors: !ModelState.IsValid,
                ValidationErrors: ModelState.Values
                .SelectMany(state => state.Errors)
                .Select(error => error.ErrorMessage)
                );
            return View(model); //visa model bound thing
        }

        public IActionResult ProductsThatCostMoreThan(decimal? price)
        {
            if (!price.HasValue) 
            {
                return BadRequest("You must pass a product price in the query string," +
                    "for example, /Home/ProductsThatCostMoreThan?price=50");
            }

            IEnumerable<Product> model = db.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.UnitPrice > price);

            if (!model.Any()) 
            {
                return NotFound($"No products cost more than {price:C}.");
            }

            ViewData["MaxPrice"] = price.Value.ToString("C");

            return View(model);
        }

        public async Task<IActionResult> Customers(string country)
        {
            string uri;
            if (string.IsNullOrEmpty(country))
            {
                ViewData["Title"] = "All Customers Worldwide";
                uri = "api/customers";
            }
            else
            {
                ViewData["Title"] = $"Customers in {country}";
                uri = $"api/customers/?country={country}";
            }
            HttpClient client = _clientFactory.CreateClient(
                name: "Northwind.WebApi");

            HttpRequestMessage request = new(
                method: HttpMethod.Get, requestUri: uri);

            HttpResponseMessage response = await client.SendAsync(request);

            IEnumerable<Customer>? model = await response.Content
                .ReadFromJsonAsync<IEnumerable<Customer>>();

            return View(model);
        }
    }
}
