using Concertation.Banking.API.Features.Accounts.Entities;
using Concertation.Banking.API.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Concertation.Banking.API.Infrastructure.Database;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new AppDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

        if (context.Roles.Any())
            return;

        context.Roles.Add(new Role { Id = Guid.CreateVersion7(), Name = "User" });
        context.Roles.Add(new Role { Id = Guid.CreateVersion7(), Name = "Admin" });

        context.SaveChanges();
    }
}
