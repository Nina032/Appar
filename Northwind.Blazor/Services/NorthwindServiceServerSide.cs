using Microsoft.EntityFrameworkCore; // ToListAsync<T>
namespace Northwind.Blazor.Services
{
    public class NorthwindServiceServerSide : INorthwindService
    {
        private readonly NorthwindContext _db;

        public NorthwindServiceServerSide(NorthwindContext db)
        {
            _db = db;
        }

        public Task<List<Customer>> GetCustomersAsync()
        {
            return _db.Customers.ToListAsync();
        }
        //resten av tasks ska implementeras

    }
}
