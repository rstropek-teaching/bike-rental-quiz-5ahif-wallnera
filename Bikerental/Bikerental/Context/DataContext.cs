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
            optionsBuilder.UseSqlServer("Server = tcp:bikerentalserverwallnera.database.windows.net,1433;Initial Catalog = BikeRentalDB; Persist Security Info = False; User ID = wallnera; Password = [password]; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30");
        }
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> opt) : base(opt)
        {
        }
    }
}
