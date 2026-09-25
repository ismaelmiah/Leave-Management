using System;
using System.Linq;
using System.Threading.Tasks;
using Leave_Management.Data;
using Microsoft.AspNetCore.Identity;

namespace Leave_Management
{
    public static class SeedData
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<Employee> userManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
        }

        private static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
                "Administrator",
                "Employee"
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole(roleName));

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            $"Failed to create role '{roleName}': " +
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        private static async Task SeedUsersAsync(
            UserManager<Employee> userManager)
        {
            await CreateUserAsync(
                userManager,
                "admin@localhost.com",
                "Administrator");

            await CreateUserAsync(
                userManager,
                "employee@localhost.com",
                "Employee");
        }

        private static async Task CreateUserAsync(
            UserManager<Employee> userManager,
            string email,
            string role)
        {
            var user = await userManager.FindByNameAsync(email);

            if (user != null)
            {
                return;
            }

            user = new Employee
            {
                UserName = email,
                Email = email
            };

            var result = await userManager.CreateAsync(
                user,
                "P@ssword1");

            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Failed to create user '{email}': " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var roleResult = await userManager.AddToRoleAsync(
                user,
                role);

            if (!roleResult.Succeeded)
            {
                throw new Exception(
                    $"Failed to assign role '{role}' to '{email}': " +
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}