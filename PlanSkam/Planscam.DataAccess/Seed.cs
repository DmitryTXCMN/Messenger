using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Planscam.Entities;

namespace Planscam.DataAccess;

internal static class Seed
{
    public static ModelBuilder CreateRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Name = "Author",
                NormalizedName = "AUTHOR"
            },
            new IdentityRole
            {
                Name = "SUB",
                NormalizedName = "SUB"
            });
        return modelBuilder;
    }

    public static ModelBuilder CreateSubscriptions(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscription>().HasData(
            new Subscription
            {
                Id = 1,
                Name = "Month",
                Description = "Month",
                Price = 100,
                Duration = Subscription.SubscriptionDurations.Month
            },
            new Subscription
            {
                Id = 2,
                Name = "3 months",
                Description = "3 months",
                Price = 250,
                Duration = Subscription.SubscriptionDurations.ThreeMonths
            },
            new Subscription
            {
                Id = 3,
                Name = "Year",
                Description = "Year",
                Price = 800,
                Duration = Subscription.SubscriptionDurations.Year
            });
        return modelBuilder;
    }
}
