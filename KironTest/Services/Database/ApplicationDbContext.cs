using KironTest.Entities;
using Microsoft.EntityFrameworkCore;

namespace KironTest.Services.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Entities.User> Users { get; set; }

        public DbSet<Navigation> Navigation { get; set; }

        public DbSet<Holiday> Holidays { get; set; }

        public DbSet<Region> Regions { get; set; }

        public DbSet<RegionHoliday> RegionHolidays { get; set; }
    }
}
