using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Common.DataContext.SqlServer
{
    public static class NorthwindContextExtensions
    {
        /// <summary>
        /// Adds NorthwindContext to the specified IServiceCollectiona. 
        /// Uses the SqlServer database provider.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString">Set to override the default.</param>
        /// <returns> An IServiceCollection that can be used to add more services</returns>
        public static IServiceCollection AddNorthwindContext(
            this IServiceCollection services,
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Northwind;" +
            "Integrated Security=True;MultipleActiveResultsets=true;Encrypt=false")
        {
            services.AddDbContext<NorthwindContext>(options =>
            {
                options.UseSqlServer(connectionString);

                options.LogTo(Console.WriteLine,
                    new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuted });
            });

            return services;
        }
      
    }
}
