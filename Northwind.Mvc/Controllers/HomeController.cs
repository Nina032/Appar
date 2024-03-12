using Microsoft.AspNetCore.Mvc; //Controller, IActionResult
using Northwind.Mvc.Models;//ErrorViewModel
using System.Diagnostics; //Activity
using Northwind.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Authorization;

namespace Northwind.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext db;

        public HomeController(ILogger<HomeController> logger,
            NorthwindContext injectedContext)
        {
            _logger = logger;
            db = injectedContext;
        }

        public IActionResult Index()
        {
            _logger.LogError("This is a serious error");
            _logger.LogWarning("This is your first warning!");
            _logger.LogWarning("Second warning");
            _logger.LogInformation("I am in the Index method of the HomeController.");

            HomeIndexViewModel model = new
                (
                    VisitorCount: Random.Shared.Next(1,1001),
                    Categories: db.Categories.ToList(),
                    Products: db.Products.ToList()
                );

            return View(model); //pass model to view
        }
        [Authorize(Roles ="Administrators")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ProductDetail(int? id)
        {
            if(!id.HasValue)
            {
                return BadRequest("You must pass a product ID in the route, " +
                    "for example, /Home/ProductDetail/13");
            }
            Product? model = db.Products.SingleOrDefault(p => p.ProductId == id);
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
    }
}
