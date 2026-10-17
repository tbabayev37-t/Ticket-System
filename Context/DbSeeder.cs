using Microsoft.AspNetCore.Identity;
using TicketSistemi.Models;

namespace TicketSistemi.Context
{
    public sealed class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            await SeedRolesAsync(services);
            await SeedAdminAsync(services, configuration);
        }
        private static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            foreach (var role in new[] { UserRoles.User, UserRoles.Admin })
            {
                if (await roleManager.RoleExistsAsync(role))
                    continue;

                var result = await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        $"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        private static async Task SeedAdminAsync(IServiceProvider services, IConfiguration configuration)
        {
            var email = configuration["AdminSeed:Email"];
            var password = configuration["AdminSeed:Password"];

            // ayarlar yoxdursa admin yaradılmır
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            if (await userManager.FindByEmailAsync(email) is not null)
                return; 

            var admin = new ApplicationUser
            {
                FullName = "Administrator",
                UserName = "admin",
                Email = email
            };

            var createResult = await userManager.CreateAsync(admin, password);
            if (!createResult.Succeeded)
                throw new InvalidOperationException(
                    $"Admin seed failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

            await userManager.AddToRoleAsync(admin, UserRoles.Admin);
        }
    }
}
