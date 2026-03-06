using Application.Abstractions.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Infrastructure.Identity;
using System;

public static class IdentitySeeder
{
    public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<IApplicationDbContext>();

        var roles = new[] { "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        await CreateUserAsync(
            userManager, dbContext,
            email: "admin@gmail.com",
            fullName: "System Admin",
            password: "Admin@123",
            role: "Admin"
        );
    }

    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext dbContext,
        string email,
        string fullName,
        string password,
        string role)
    {
        if (await userManager.FindByEmailAsync(email) != null)
            return;

        var domainUser = User.Create(fullName, email);
        await dbContext.Users.AddAsync(domainUser);
        await dbContext.SaveChangesAsync();

        var appUser = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = fullName,
            DomainUserId = domainUser.Id  
        };

        var result = await userManager.CreateAsync(appUser, password);

        if (result.Succeeded)
            await userManager.AddToRoleAsync(appUser, role);
    }
}