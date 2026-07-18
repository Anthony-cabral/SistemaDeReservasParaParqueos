using Microsoft.EntityFrameworkCore;
using ParkRD.Domain.Entities;

namespace ParkRD.Infrastructure.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }

        public DbSet<Vehicles> Vehicles { get; set; }

        public DbSet<Parkings> Parkings { get; set; }

        public DbSet<Reservations> Reservations { get; set; }
    }
}
