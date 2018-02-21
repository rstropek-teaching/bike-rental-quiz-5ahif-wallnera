using Bikerental.Model;
using Microsoft.EntityFrameworkCore;

namespace Bikerental.Context
{
    public class DataContext:DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Bike> Bikes { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = (localdb)\\MSSQLLocalDB; Database = BikeRentalDB; Trusted_Connection = True");
        }
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> opt) : base(opt)
        {
        }
    }
}
