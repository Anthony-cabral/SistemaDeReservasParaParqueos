using Microsoft.EntityFrameworkCore;
using ParkRD.API.Models.Entities;

namespace ParkRD.API.Data
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