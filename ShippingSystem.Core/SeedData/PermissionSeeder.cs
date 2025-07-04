using Microsoft.EntityFrameworkCore;
using ShippingSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingSystem.Core.SeedData
{
    public static class PermissionSeeder
    {
        public static async Task SeedPermissions(ShippingContext context)
        {
            if (!context.Permissions.Any())
            {
                var perms = new[]
                {
                new Permission { Name = "ManageUsers" },
                new Permission { Name = "CreateOrder" },
                new Permission { Name = "EditOrder" },
                new Permission { Name = "DeleteOrder" },
            };

                await context.Permissions.AddRangeAsync(perms);
                await context.SaveChangesAsync();
            }

            if (!context.Groups.Any())
            {
                var admin = new Group { Name = "Admins" };
                await context.Groups.AddAsync(admin);
                await context.SaveChangesAsync();

                var allPerms = await context.Permissions.ToListAsync();
                foreach (var perm in allPerms)
                {
                    await context.GroupPermissions.AddAsync(new GroupPermission
                    {
                        GroupId = admin.Id,
                        PermissionId = perm.Id
                    });
                }

                await context.SaveChangesAsync();
            }
        }
    }

}
