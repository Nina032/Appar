using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using Northwind.Shared;

namespace Northwind.WebApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly MemoryCacheEntryOptions _cacheEntryOptions = new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(30)
        };

        private NorthwindContext _db;

        public CustomerRepository(NorthwindContext db,
            IMemoryCache memoryCache)
        {
            _db = db;
            _memoryCache = memoryCache;
        }

        public async Task<Customer?> CreateAsync(Customer c)
        {
            c.CustomerId = c.CustomerId.ToUpper();

            //Lägg till post i db med EF Core
            EntityEntry<Customer> added = await _db.Customers.AddAsync(c);
            int affected = await _db.SaveChangesAsync();
            //Om den sparas spara cache
            if (affected == 1)
            {
                _memoryCache.Set(c.CustomerId, c, _cacheEntryOptions);
                return c;
            }
            return null;
        }

        public async Task<bool?> DeleteAsync(string id)
        {
            id = id.ToUpper();

            Customer? c = await _db.Customers.FindAsync(id);
            if (c is null) return null;

            _db.Customers.Remove(c);
            int affected = await _db.SaveChangesAsync();
            if(affected == 1)
            {
                _memoryCache.Remove(c.CustomerId);
                return true;
            }
            return null;
        }

        public Task<Customer[]> RetrieveAllAsync()
        {
            return _db.Customers.ToArrayAsync();
        }

        public Task<Customer?> RetrieveAsync(string id)
        {
            id = id.ToUpper();
            //Kolla om vi har cache först
            if (_memoryCache.TryGetValue(id, out Customer? fromCache))
                return Task.FromResult(fromCache);

            // Saknas i cache -- hämta från DB
            Customer? fromDb = _db.Customers.FirstOrDefault
                (c => c.CustomerId == id);

            //Om post saknas i DB returnera null
            if (fromDb is null) return Task.FromResult(fromDb);

            //Om post finns i db, då lagra den i cache och returnera
            _memoryCache.Set(fromDb.CustomerId, fromDb, _cacheEntryOptions);
            return Task.FromResult(fromDb)!;
        }

        public async Task<Customer?> UpdateAsync(Customer c)
        {
            c.CustomerId = c.CustomerId.ToUpper();

            _db.Customers.Update(c);
            int affected = await _db.SaveChangesAsync();

            if (affected == 1)
            {
                _memoryCache.Set(c.CustomerId, c, _cacheEntryOptions);
                return c;
            }
            return null;
        }
    }
}
