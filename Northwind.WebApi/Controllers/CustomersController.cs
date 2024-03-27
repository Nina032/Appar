using Microsoft.AspNetCore.Mvc; //[Route] [ApiController], ControllerBase ...
using Northwind.Shared; //För Customer model
using Northwind.WebApi.Repositories; // ICustomerRepository

namespace Northwind.WebApi.Controllers
{
    // api/customers
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _repo;

        public CustomersController(ICustomerRepository repo)
        {
            _repo = repo;
        }

        //GET:  api/customers
        //GET:  api/customers/?country=[country]
        // returnerar lista med customers (men den kan vara null)
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        public async Task<IEnumerable<Customer>> GetCustomers(string? country)
        {
            if(string.IsNullOrWhiteSpace(country))
            {
                return await _repo.RetrieveAllAsync();
            }
            else
            {
                return (await _repo.RetrieveAllAsync())
                    .Where(customer => customer.Country == country);
            }
        }
    }
}
