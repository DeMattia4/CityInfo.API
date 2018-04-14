using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class CityInfoContext : DbContext
    {
        // Constructor to create DB Context
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            : base(options)
        {
            // Ensure db is created if it doesn't already exist
            Database.Migrate();
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }
    }
}
