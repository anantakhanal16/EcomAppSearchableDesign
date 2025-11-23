using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.SeedData
{
    public class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.Roles.AnyAsync()) return;

            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}