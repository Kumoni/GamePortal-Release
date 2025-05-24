using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Game.Models;

namespace Game.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            try
            {
                await context.Database.EnsureCreatedAsync();

                // Админ
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                var adminUser = await userManager.FindByEmailAsync("admin@example.com");
                if (adminUser == null)
                {
                    adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com" };
                    await userManager.CreateAsync(adminUser, "AdminPassword123!");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                // Уровни
                if (!context.Levels.Any())
                {
                    context.Levels.AddRange(
                        new Level { Name = "Лес", Difficulty = 2 },
                        new Level { Name = "Пещера", Difficulty = 4 },
                        new Level { Name = "Замок", Difficulty = 7 }
                    );
                    await context.SaveChangesAsync();
                }

                // Игроки
                if (!context.Players.Any())
                {
                    context.Players.AddRange(
                        new Player { Name = "Иван", RegistrationDate = DateTime.Now.AddDays(-10) },
                        new Player { Name = "Мария", RegistrationDate = DateTime.Now.AddDays(-5) },
                        new Player { Name = "Алексей", RegistrationDate = DateTime.Now }
                    );
                    await context.SaveChangesAsync();
                }

                // Персонажи
                if (!context.Characters.Any())
                {
                    var player1 = context.Players.FirstOrDefault(p => p.Name == "Иван");
                    var player2 = context.Players.FirstOrDefault(p => p.Name == "Мария");
                    var player3 = context.Players.FirstOrDefault(p => p.Name == "Алексей");
                    var level1 = context.Levels.FirstOrDefault(l => l.Name == "Лес");
                    var level2 = context.Levels.FirstOrDefault(l => l.Name == "Пещера");
                    var level3 = context.Levels.FirstOrDefault(l => l.Name == "Замок");

                    context.Characters.AddRange(
                        new Character { Name = "Рыцарь", Class = "Воин", LevelId = 5, Player = player1, Level = level1 },
                        new Character { Name = "Лучница", Class = "Охотник", LevelId = 3, Player = player2, Level = level2 },
                        new Character { Name = "Маг", Class = "Волшебник", LevelId = 7, Player = player3, Level = level3 }
                    );
                    await context.SaveChangesAsync();
                }
            }
            finally
            {
                if (context is IDisposable disposableContext)
                {
                    disposableContext.Dispose();
                }
            }
        }
    }
}