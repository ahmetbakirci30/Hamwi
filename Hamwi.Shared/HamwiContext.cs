using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Hamwi.Shared.Entities;
using Hamwi.Shared.Entities.Statuses;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hamwi.Shared
{
    public class HamwiContext : IdentityDbContext
    {
        public HamwiContext(DbContextOptions options) : base(options) { }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.State.Equals(EntityState.Added) || entry.State.Equals(EntityState.Modified)))
                entry.Property("Date").CurrentValue = DateTime.Now;

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public DbSet<Video> Videos { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}