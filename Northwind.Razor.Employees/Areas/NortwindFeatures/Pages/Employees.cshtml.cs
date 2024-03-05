using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Northwind.Shared;

namespace NorthwindFeatures.Pages
{
    public class EmployeesPageModel : PageModel
    {
        private NorthwindContext db;

        public EmployeesPageModel(NorthwindContext injectedContext)
        {
            db = injectedContext;
        }

        public Employee[] Employees { get; set; } = null!;
        public void OnGet()
        {
            ViewData["Title"] = "Nortwind B2B - Employees";
            
            Employees = db.Employees.OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName).ToArray();

        }
    }
}
