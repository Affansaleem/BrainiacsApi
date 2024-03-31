using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BrainiacsApi.Data
{
    public class BrainiacsDbContext:IdentityDbContext<IdentityUser>
    {
        public BrainiacsDbContext(DbContextOptions options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            SeedRoles(builder);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole(){Name="Admin",NormalizedName="Admin",ConcurrencyStamp="1"},
                new IdentityRole(){Name="User",NormalizedName="User",ConcurrencyStamp="2"}
                // new IdentityRole(){Name="Admin",NormalizedName="Admin",ConcurrencyStamp="1"}
            );
        }
    }
}