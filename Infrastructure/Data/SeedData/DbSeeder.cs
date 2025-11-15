using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.SeedData
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            await SupplierSeeder.SeedAsync(context);
        }
    }
}