using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.SeedData;

public static class SupplierSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (!await context.Suppliers.AnyAsync())
        {
            var suppliers = new List<Supplier>
            {
                new Supplier
                {
                    SupplierName = "Global Traders",
                    Email = "contact@globaltraders.com",
                    City = "Kathmandu",
                    Country = "Nepal"
                },
                new Supplier
                {
                    SupplierName = "Himalayan Exports",
                    Email = "info@himalayanexports.com",
                    City = "Pokhara",
                    Country = "Nepal"
                },
                new Supplier
                {
                    SupplierName = "Tech Supplies Ltd",
                    Email = "support@techsupplies.com",
                    City = "Delhi",
                    Country = "India"
                },
                new Supplier
                {
                    SupplierName = "Everest Wholesalers",
                    Email = "sales@everestwholesalers.com",
                    City = "Bhaktapur",
                    Country = "Nepal"
                }
            };

            await context.Suppliers.AddRangeAsync(suppliers);
            await context.SaveChangesAsync();
        }
    }
}