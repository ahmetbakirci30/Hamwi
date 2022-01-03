using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hamwi.Shared.Managers.Files;
using Hamwi.Shared.Managers.Files.Interfaces;
using Hamwi.Shared.Repositories;
using Hamwi.Shared.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Hamwi.Shared.Extensions.Startup
{
    public static class StartupExtensions
    {
        public static void AddRequiredServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HamwiContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IFileManager), typeof(FileManager));
        }

        public static async Task SeedDataAsync(this IApplicationBuilder app, IConfiguration configuration)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            using var dbContext = serviceProvider.GetService<HamwiContext>();

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                await dbContext.Database.MigrateAsync();

                var email = configuration["AdminInfo:UserName"];
                var phone = configuration["AdminInfo:PhoneNumber"];
                var password = configuration["AdminInfo:Password"];
                var admin = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    PhoneNumber = phone
                };

                using var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
                await userManager.CreateAsync(admin, password);
            }
        }
    }
}